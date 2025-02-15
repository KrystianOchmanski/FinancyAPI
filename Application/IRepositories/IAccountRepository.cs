﻿namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<IEnumerable<Account>> GetAllAccountsByUserIdAsync(string userId);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task UpdateAccountBalanceAsync(int accountId, decimal newBalance);
        Task DeleteAccountAsync(int accountId);
    }
}
