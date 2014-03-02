﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using POEApi.Infrastructure;
using POEApi.Infrastructure.Events;
using POEApi.Model;
using POEApi.Model.Events;
using Procurement.View;

namespace Procurement.ViewModel
{
    public class LoginWindowViewModel : INotifyPropertyChanged
    {
        private static bool authOffLine;

        private LoginView view = null;
        private StatusController statusController;
        public event LoginCompleted OnLoginCompleted;
        public delegate void LoginCompleted();
        private bool formChanged = false;
        private bool useSession;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private string email;
        public string Email 
        {
            get { return email; }
            set
            {
                if (value != email)
                {
                    email = value;
                    OnPropertyChanged("Email");
                }
            }
        }

        public bool UseSession
        {
            get { return useSession; }
            set 
            { 
                useSession = value;                
                Settings.UserSettings["UseSessionID"] = value.ToString();
                updateButtonLabels(useSession);
            }
        }

        private void updateButtonLabels(bool useSession)
        {
            if (this.view == null)
                return;

            this.view.lblEmail.Content = useSession ? "Alias" : "Email";
            this.view.lblPassword.Content = useSession ? "Session ID" : "Password";
        }

        public LoginWindowViewModel(UserControl view)
        {
            this.view = view as LoginView;

            UseSession = Settings.UserSettings.ContainsKey("UseSessionID") ? bool.Parse(Settings.UserSettings["UseSessionID"]) : false;

            Email = Settings.UserSettings["AccountLogin"];
            this.formChanged = string.IsNullOrEmpty(Settings.UserSettings["AccountPassword"]);

            if (!this.formChanged)
                this.view.txtPassword.Password = string.Empty.PadLeft(8); //For the visuals

            this.view.txtPassword.PasswordChanged += new System.Windows.RoutedEventHandler(txtPassword_PasswordChanged);

            statusController = new StatusController(this.view.StatusBox);
            statusController.DisplayMessage(ApplicationState.Version + " Initialized.\r");

            ApplicationState.Model.Authenticating += new POEModel.AuthenticateEventHandler(model_Authenticating);
            ApplicationState.Model.StashLoading += new POEModel.StashLoadEventHandler(model_StashLoading);
            ApplicationState.Model.ImageLoading += new POEModel.ImageLoadEventHandler(model_ImageLoading);
            ApplicationState.Model.Throttled += new ThottledEventHandler(model_Throttled);
            ApplicationState.InitializeFont(Properties.Resources.fontin_regular_webfont);
            ApplicationState.InitializeFont(Properties.Resources.fontin_smallcaps_webfont);
        }

        void txtPassword_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            this.formChanged = true;
        }

        public void Login(bool offline)
        {
            authOffLine = offline;
            toggleControls();

            Task.Factory.StartNew(() =>
            {
                SecureString password = formChanged ? this.view.txtPassword.SecurePassword : Settings.UserSettings["AccountPassword"].Decrypt();
                ApplicationState.Model.Authenticate(Email, password, authOffLine, useSession);
                saveSettings(password);

                if (!authOffLine)
                    ApplicationState.Model.ForceRefresh();

                statusController.DisplayMessage("Loading characters...");
                List<Character> chars;
                try
                {
                    chars = ApplicationState.Model.GetCharacters();
                } 
                catch (WebException wex) 
                {
                    Logger.Log(wex);                    
                    statusController.NotOK();
                    throw new Exception("Failed to load characters", wex.InnerException);
                }
                statusController.Ok();

                bool downloadOnlyMyLeagues = false;
                downloadOnlyMyLeagues = (Settings.UserSettings.ContainsKey("DownloadOnlyMyLeagues") && 
                                         bool.TryParse(Settings.UserSettings["DownloadOnlyMyLeagues"], out downloadOnlyMyLeagues) && 
                                         downloadOnlyMyLeagues &&
                                         Settings.Lists.ContainsKey("MyLeagues") &&
                                         Settings.Lists["MyLeagues"].Count > 0
                                         );

                foreach (var character in chars)
                {
                    if (character.League == "Void")
                        continue;

                    if (downloadOnlyMyLeagues && !Settings.Lists["MyLeagues"].Contains(character.League))
                        continue;

                    ApplicationState.Characters.Add(character);
                    loadCharacterInventory(character);
                    loadStash(character);
                }

                if (downloadOnlyMyLeagues && ApplicationState.Characters.Count == 0)
                    throw new Exception("No characters found in the leagues specified. Check spelling or try setting DownloadOnlyMyLeagues to false in settings");

                ApplicationState.SetDefaults();

                statusController.DisplayMessage("\nDone!");
                OnLoginCompleted();
            }).ContinueWith((t) => { Logger.Log(t.Exception.InnerException.ToString()); statusController.HandleError(t.Exception.InnerException.Message, toggleControls); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void saveSettings(SecureString password)
        {
            if (!formChanged)
                return;
         
            Settings.UserSettings["AccountLogin"] = Email;
            Settings.UserSettings["AccountPassword"] = password.Encrypt();
            Settings.UserSettings["UseSessionID"] = useSession.ToString();
            Settings.Save();
        }

        private void toggleControls()
        {
            view.LoginButton.IsEnabled = !view.LoginButton.IsEnabled;
            view.OfflineButton.IsEnabled = !view.OfflineButton.IsEnabled;
            view.txtLogin.IsEnabled = !view.txtLogin.IsEnabled;
            view.txtPassword.IsEnabled = !view.txtPassword.IsEnabled;
        }

        private void loadStash(Character character)
        {
            if (ApplicationState.Leagues.Contains(character.League))
                return;

            ApplicationState.CurrentLeague = character.League;
            ApplicationState.Stash[character.League] = ApplicationState.Model.GetStash(character.League);
            ApplicationState.Model.GetImages(ApplicationState.Stash[character.League]);
            ApplicationState.Leagues.Add(character.League);
        }

        private void loadCharacterInventory(Character character)
        {
            bool success = false;
            statusController.DisplayMessage((string.Format("Loading {0}'s inventory...", character.Name)));
            List<Item> inventory = null;
            try
            {
                inventory = ApplicationState.Model.GetInventory(character.Name);
                success = true;
            }
            catch (WebException)
            {
                inventory = new List<Item>();
                success = false;
            }

            var inv = inventory.Where(i => i.inventoryId != "MainInventory");
            if (success)
                statusController.Ok();
            else
                statusController.NotOK();

            ApplicationState.Model.GetImages(inventory);
        }

        void model_StashLoading(POEModel sender, StashLoadedEventArgs e)
        {
            update("Loading " + ApplicationState.CurrentLeague + " Stash Tab " + (e.StashID + 1) + "...", e);
        }

        void model_ImageLoading(POEModel sender, ImageLoadedEventArgs e)
        {
            update("Loading Image For " + e.URL, e);
        }

        void model_Authenticating(POEModel sender, AuthenticateEventArgs e)
        {
            update("Authenticating " + e.Email, e);
        }

        void model_Throttled(object sender, ThottledEventArgs e)
        {
            if (e.WaitTime.TotalSeconds > 4)
                update(string.Format("GGG Server request limit hit, throttling activated. Please wait {0} seconds", e.WaitTime.Seconds), new POEEventArgs(POEEventState.BeforeEvent));
        }

        private void update(string message, POEEventArgs e)
        {
            if (e.State == POEEventState.BeforeEvent)
            {
                statusController.DisplayMessage(message);
                return;
            }

            statusController.Ok();
        }
    }
}