using MyFinances.Data.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Data
{
	public class UserAccountService
	{
		private List<UserAccount> _userAccountList;
		private List<LoanCalculation> _loanCalculationsList;
		protected readonly ApplicationDbContext _dbcontext;

		public UserAccountService(ApplicationDbContext dbcontext)
		{
			_dbcontext = dbcontext;
			RefreshUserAccounts();
			RefreshLoanCalculations();
		}

		public void RefreshUserAccounts()
		{
			_userAccountList = _dbcontext.user.ToList();
		}

		public void RefreshLoanCalculations()
		{
			_loanCalculationsList = _dbcontext.loan_calculation.ToList();
		}

		public List<UserAccount> GetUserAccounts()
		{
			return _userAccountList;
		}

		public Task<UserAccount> GetUserAccountByUsername(string username)
		{
			var user = _userAccountList.Where(a => a.username == username).FirstOrDefault();

			if (user != null)
				return Task.FromResult(user);
			else
				throw new Exception($"Username: {username} doesn't exist");
		}

		public Task<UserAccount> GetUserAccountByUserAndPassword(string userName, string password)
		{
			var user = _userAccountList.Where(a => a.username == userName && a.password == password).FirstOrDefault();

			if (user != null)
			{
				user.last_login = DateTime.Now;
				_dbcontext.SaveChangesAsync();
				return Task.FromResult(user);
			}
			else
				throw new Exception("Invalid username or password");
		}

		public List<LoanCalculation> GetLoanCalculations(string userName)
		{
			if (userName != null)
				return _loanCalculationsList.Where(a => a.fk_user_id == _userAccountList.Where(a => a.username == userName).FirstOrDefault().id).ToList();
			else
				return new List<LoanCalculation>();
		}

		public Task<bool> InsertUserAccount(UserAccount user)
		{
			try
			{
				user.role = "user";
				user.created = DateTime.UtcNow;
				_dbcontext.user.Add(user);
				_dbcontext.SaveChanges();
				RefreshUserAccounts();
				return Task.FromResult(true);
			}
			catch (Exception)
			{
				return Task.FromResult(false);
			}
		}

		public Task<List<string>> UpdateUserAccount(UserAccount user, UserAccount admin = null)
		{
			var result = new List<string>();
			try
			{
				UserAccount userDB = null;

				if (admin != null && admin.role == "admin")
				{
					userDB = _userAccountList.FirstOrDefault(a => a.id == user.id);
					userDB.fullname = user.fullname;
					userDB.email = user.email;
					userDB.password = user.password;
					userDB.modified = DateTime.UtcNow;
					userDB.modified_by = admin.username;
					_dbcontext.SaveChanges();
					RefreshUserAccounts();
				}
				else
				{
					userDB = _userAccountList.FirstOrDefault(a => a.username == user.username);

					if (!Equals(userDB.email, user.email) && user.email != null)
					{
						userDB.email = user.email;
						result.Add("E-mail");
					}

					if (!Equals(userDB.password, user.password) && user.password != String.Empty && user.password != null)
					{
						userDB.password = user.password;
						result.Add("Hasło");
					}

					if (result.Count > 0)
					{
						userDB.modified = DateTime.UtcNow;
						userDB.modified_by = userDB.username;
						_dbcontext.SaveChanges();
						RefreshUserAccounts();
					}
				}
				return Task.FromResult(result);
			}
			catch (Exception)
			{
				result.Clear();
				return Task.FromResult(result);
			}
		}

		public Task<bool> DeleteUserAccount(UserAccount user, UserAccount admin = null)
		{
			try
			{
				var userDB = _userAccountList.FirstOrDefault(a => a.username == user.username);
				DeleteLoanCalculations(GetLoanCalculations(userDB.username));
				_dbcontext.user.Remove(userDB);
				var result = _dbcontext.SaveChanges();
				RefreshUserAccounts();
				return Task.FromResult(result > 0);
			}
			catch (Exception)
			{
				return Task.FromResult(false);
			}
		}

		public bool InsertLoanCalculation(LoanCalculation loanCalculation, string username)
		{
			var user = _userAccountList.FirstOrDefault(a => a.username == username);
			loanCalculation.fk_user_id = user.id;
			_dbcontext.loan_calculation.Add(loanCalculation);
			_dbcontext.SaveChanges();
			RefreshLoanCalculations();
			return true;
		}

		public bool UpdateLoanCalculation(LoanCalculation loanCalculation)
		{
			var loanCalculationUpdate = _dbcontext.loan_calculation.FirstOrDefault(a => a.iid == loanCalculation.iid);
			if (loanCalculationUpdate != null)
			{
				loanCalculationUpdate.percentage = loanCalculation.percentage;
				loanCalculationUpdate.amount = loanCalculation.amount;
				loanCalculationUpdate.duration = loanCalculation.duration;
				_dbcontext.SaveChanges();
				RefreshLoanCalculations();

				return true;
			}
			return false;
		}

		public bool DeleteLoanCalculation(LoanCalculation loanCalculation, bool refresh = true)
		{
			_dbcontext.loan_calculation.Remove(loanCalculation);
			_dbcontext.SaveChanges();

			if (refresh)
				RefreshLoanCalculations();

			return true;
		}

		public bool DeleteLoanCalculations(List<LoanCalculation> loanCalculations)
		{
			bool result = false;

			foreach (var loanCalculation in loanCalculations)
			{
				result |= DeleteLoanCalculation(loanCalculation, false);
			}

			if (result)
				RefreshLoanCalculations();
			return result;
		}
	}
}
