﻿using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers
{
    /// <summary>
    /// URL: https://localhost:6600/api/expense-tracker/
    /// </summary>
    [Route(RouteConstants.BasePath)]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseTrackerContext context;

        public ExpenseController(ExpenseTrackerContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// URL: https://localhost:6600/api/expense-tracker/expenses
        /// </summary>
        [HttpGet]
        [Route(RouteConstants.Expenses)]
        public async Task<IActionResult> ReadExpenses()
        {
            try
            {
                var expenses = await context.Expenses
                    .AsNoTracking()
                    .OrderBy(e => e.ExpenseDate)
                    .ToListAsync();

                return Ok(expenses);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// URL: https://localhost:6600/api/expense-tracker/expenses/{key}
        /// </summary>
        /// <param name="key">Primary key of expense entity.</param>
        [HttpGet]
        [Route(RouteConstants.Expenses + "{key}")]
        public async Task<IActionResult> ReadExpenseByKey(int key)
        {
            try
            {
                if (key <= 0)
                    return StatusCode(StatusCodes.Status400BadRequest);

                var expense = await context.Expenses.FindAsync(key);

                if (expense == null)
                    return StatusCode(StatusCodes.Status404NotFound);

                return Ok(expense);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// URL: https://localhost:6600/api/expense-tracker/expenses/create
        /// </summary>
        /// <param name="expense">Expense object.</param>>
        [HttpPost]
        [Route(RouteConstants.CreateExpense)]
        public async Task<IActionResult> CreateExpense(Expense expense)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest);

                if (!IsValidExpense(expense))
                    return StatusCode(StatusCodes.Status400BadRequest);

                context.Expenses.Add(expense);
                await context.SaveChangesAsync();
                return CreatedAtAction("ReadExpenseByKey", new { key = expense.CategoryID }, expense);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// URL: https://localhost:6600/api/expense-tracker/expenses/update
        /// </summary>
        /// <param name="id">Primary key of the expense entity.</param>
        /// <param name="expense">Expense object.</param>
        [HttpPut]
        [Route(RouteConstants.UpdateExpense)]
        public async Task<IActionResult> UpdateExpense(int id, Expense expense)
        {
            try
            {
                if (id != expense.ExpenseID)
                    return StatusCode(StatusCodes.Status400BadRequest);

                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status400BadRequest);

                if (!IsValidExpense(expense))
                    return StatusCode(StatusCodes.Status400BadRequest);

                if (!await IsExpenseExistant(id))
                    return StatusCode(StatusCodes.Status404NotFound);

                context.Entry(expense).State = EntityState.Modified;
                context.Expenses.Update(expense);
                await context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// URL: https://localhost:6600/api/expense-tracker/expenses/delete/{key}
        /// </summary>
        /// <param name="key">Primary key of the expense entity.</param>
        [HttpDelete]
        [Route(RouteConstants.DeleteExpense + "{key}")]
        public async Task<IActionResult> DeleteExpense(int key)
        {
            try
            {
                if (key <= 0)
                    return StatusCode(StatusCodes.Status400BadRequest);

                var expense = await context.Expenses.FindAsync(key);

                if (expense == null)
                    return StatusCode(StatusCodes.Status404NotFound);

                context.Expenses.Remove(expense);
                await context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Verifying whether the expense date is a future date or if the amount is less than or equal to zero.
        /// </summary>
        /// <param name="expense">Expense object.</param>
        /// <returns>Boolean</returns>
        private static bool IsValidExpense(Expense expense)
        {
            if (expense.ExpenseDate > DateTime.Now || expense.Amount <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Verifying whether the expense is existant or not.
        /// </summary>
        /// <param name="id">Primary key of the expense entity.</param>
        /// <returns>Boolean</returns>
        private async Task<bool> IsExpenseExistant(int id)
        {
            try
            {
                var expenseInDb = await context.Expenses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.ExpenseID == id);

                if (expenseInDb == null)
                    return false;

                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}