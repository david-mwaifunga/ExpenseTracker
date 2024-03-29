﻿namespace ExpenseTracker.Utilities
{
    public static class RouteConstants
    {
        /// <summary>
        /// Api BasePath.
        /// </summary>
        public const string BasePath = "api/expense-tracker/";

        /// <summary>
        /// CategoryController routes.
        /// </summary>
        public const string Categories = "categories/";
        public const string CreateCategory = "categories/create";
        public const string UpdateCategory = "categories/update";
        public const string DeleteCategory = "categories/delete/";

        /// <summary>
        /// ExpenseController routes.
        /// </summary>
        public const string Expenses = "expenses/";
        public const string CreateExpense = "expenses/create";
        public const string UpdateExpense = "expenses/update";
        public const string DeleteExpense = "expenses/delete/";
    }
}