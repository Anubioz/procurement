﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POEApi.Model;

namespace Procurement.ViewModel.Recipes
{
    public abstract class Recipe
    {
        public abstract string Name { get; }
        protected bool ReturnsPartialMatches { get; private set; }
        protected decimal ReturnMatchesGreaterThan { get; private set; }

        public Recipe()
        {
            ReturnsPartialMatches = false;
        }
        public Recipe(decimal returnMatchesGreaterThan)
        {
            this.ReturnsPartialMatches = true;
            this.ReturnMatchesGreaterThan = returnMatchesGreaterThan;
        }

        public virtual string GetResultName(RecipeResult result)
        {
            return Name;
        }

        public abstract IEnumerable<RecipeResult> Matches(IEnumerable<Item> items);
    }
}
