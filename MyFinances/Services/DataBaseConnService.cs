using Microsoft.Extensions.Configuration;
using MyFinances.Data.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.Data
{
    public class DataBaseConnService
    {
        private List<UserAccount> _userAccountList;
        private List<LoanCalculation> _loanCalculationsList;
        private List<Configuration> _configuration;
        protected readonly ApplicationDbContext _dbcontext;

        private readonly IConfiguration _appSettings;

        public DataBaseConnService(ApplicationDbContext dbcontext, IConfiguration appSettings)
        {
            _appSettings = appSettings;
            var databaseAvailable = _appSettings.GetValue<bool>("Database:Available");
            if (databaseAvailable)
            {
                _dbcontext = dbcontext;
                RefreshUserAccounts();
                RefreshLoanCalculations();
                RefreshConfiguration();
            }
        }

        public void RefreshUserAccounts()
        {
            _userAccountList = _dbcontext.user.ToList();
        }

        public void RefreshLoanCalculations()
        {
            _loanCalculationsList = _dbcontext.loan_calculation.ToList();
        }

        public void RefreshConfiguration()
        {
            _configuration = _dbcontext.configuration.ToList();
            LoadConfiguration();
        }

        public List<UserAccount> GetUserAccounts()
        {
            return _userAccountList;
        }

        public List<Configuration> GetConfiguration()
        {
            return _configuration;
        }

        public Task<UserAccount> GetUserAccount(Guid id)
        {
            var user = _userAccountList.Where(a => a.id == id).FirstOrDefault();

            if (user != null)
                return Task.FromResult(user);
            else
                throw new Exception($"User id: {id} doesn't exist");
        }

        public Task<UserAccount> GetUserAccountByUsername(string username)
        {
            var user = _userAccountList.Where(a => a.username == username).FirstOrDefault();

            if (user != null)
                return Task.FromResult(user);
            else
                throw new Exception($"Username: {username} doesn't exist");
        }

        public bool isUsernameExist(string username)
        {
            var user = _userAccountList.Where(a => a.username == username).FirstOrDefault();

            return user != null ? true : false;
        }

        public Task<UserAccount> GetUserAccountByUsernameAndHash(string userName, string hashedPassword)
        {
            var user = _userAccountList.Where(a => a.username == userName && a.password == hashedPassword).FirstOrDefault();

            if (user != null)
            {
                user.last_login = DateTime.UtcNow;
                _dbcontext.SaveChanges();
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
                user.password = Helpers.Helper.ComputeHash(user.password);
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
                    userDB.password = Helpers.Helper.ComputeHash(user.password);
                    userDB.modified = DateTime.UtcNow;
                    userDB.modified_by = admin.username;
                    _dbcontext.SaveChanges();
                    RefreshUserAccounts();
                }
                else
                {
                    userDB = GetUserAccount(user.id).Result;

                    if (!Equals(userDB.username, user.username) && user.username != null)
                    {
                        userDB.username = user.username;
                        result.Add("Nazwa użytkownika");
                    }

                    if (!Equals(userDB.email, user.email) && user.email != null)
                    {
                        userDB.email = user.email;
                        result.Add("E-mail");
                    }

                    if (!Equals(userDB.password, Helpers.Helper.ComputeHash(user.password)) && user.password != String.Empty && user.password != null)
                    {
                        userDB.password = Helpers.Helper.ComputeHash(user.password);
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
                RefreshUserAccounts();
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

        private bool DeleteLoanCalculations(List<LoanCalculation> loanCalculations)
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

        public void LoadConfiguration()
        {
            if (_configuration.FirstOrDefault(a => a.name.Equals("Debenture.Amount")) != null)
            {
                if (int.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Debenture.Amount")).value, out int val))
                    Helpers.DefaultValue.Debenture.Amount = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("OTSPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("OTSPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.OTSPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("DOSPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("DOSPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.DOSPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("TOSPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("TOSPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.TOSPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("RORPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("RORPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.RORPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("DORPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("DORPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.DORPercentage = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("EDOAdditionalPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("EDOAdditionalPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.EDOAdditionalPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("COIAdditionalPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("COIAdditionalPercentage")).value, out double val))
                    Helpers.DefaultValue.Debenture.COIAdditionalPercentage = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionCOI")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionCOI")).value, out double val))
                    Helpers.DefaultValue.Debenture.EarlyRedemptionCOI = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionEDO")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionEDO")).value, out double val))
                    Helpers.DefaultValue.Debenture.EarlyRedemptionEDO = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionTOS")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("EarlyRedemptionTOS")).value, out double val))
                    Helpers.DefaultValue.Debenture.EarlyRedemptionTOS = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("TOZPercentage")) != null)
            {
                var val = _configuration.FirstOrDefault(a => a.name.Equals("TOZPercentage")).value;
                var TOZPercentage = new List<double>();
                foreach (var item in val.Split(','))
                {
                    if (double.TryParse(item, out double result))
                        TOZPercentage.Add(result);
                }

                if (TOZPercentage.Count == 6)
                    Helpers.DefaultValue.Debenture.TOZPercentage = val.Split(',').Select(a => double.Parse(a)).ToList();
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("EDOPercentage")) != null)
            {
                var val = _configuration.FirstOrDefault(a => a.name.Equals("EDOPercentage")).value;
                var EDOPercentage = new List<double>();
                foreach (var item in val.Split(','))
                {
                    if (double.TryParse(item, out double result))
                        EDOPercentage.Add(result);
                }

                if (EDOPercentage.Count == 10)
                    Helpers.DefaultValue.Debenture.EDOPercentage = val.Split(',').Select(a => double.Parse(a)).ToList();
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("COIPercentage")) != null)
            {
                var val = _configuration.FirstOrDefault(a => a.name.Equals("COIPercentage")).value;
                var COIPercentage = new List<double>();
                foreach (var item in val.Split(','))
                {
                    if (double.TryParse(item, out double result))
                        COIPercentage.Add(result);
                }

                if (COIPercentage.Count == 4)
                    Helpers.DefaultValue.Debenture.COIPercentage = val.Split(',').Select(a => double.Parse(a)).ToList();
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("Loan.Amount")) != null)
            {
                if (long.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Loan.Amount")).value, out long val))
                    Helpers.DefaultValue.Loan.Amount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("Loan.Percentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Loan.Percentage")).value, out double val))
                    Helpers.DefaultValue.Loan.Percentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("Loan.Duration")) != null)
            {
                if (int.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Loan.Duration")).value, out int val))
                    Helpers.DefaultValue.Loan.Duration = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Amount")) != null)
            {
                if (long.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Amount")).value, out long val))
                    Helpers.DefaultValue.Deposit.Amount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Percentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Percentage")).value, out double val))
                    Helpers.DefaultValue.Deposit.Percentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Duration")) != null)
            {
                if (int.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Duration")).value, out int val))
                    Helpers.DefaultValue.Deposit.Duration = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Period")) != null)
            {
                if (int.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("Deposit.Period")).value, out int val))
                    Helpers.DefaultValue.Deposit.Period = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.EmployerPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.EmployerPercentage")).value, out double val))
                    Helpers.DefaultValue.PPKCalc.EmployeePercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.EmployerPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.EmployerPercentage")).value, out double val))
                    Helpers.DefaultValue.PPKCalc.EmployerPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.DepositPercentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.DepositPercentage")).value, out double val))
                    Helpers.DefaultValue.PPKCalc.DepositPercentage = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.Amount")) != null)
            {
                if (long.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.Amount")).value, out long val))
                    Helpers.DefaultValue.PPKCalc.Amount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.Duration")) != null)
            {
                if (int.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKCalc.Duration")).value, out int val))
                    Helpers.DefaultValue.PPKCalc.Duration = val;
            }

            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.Amount")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.Amount")).value, out double val))
                    Helpers.DefaultValue.PPKPayout.Amount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.CountryAmount")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.CountryAmount")).value, out double val))
                    Helpers.DefaultValue.PPKPayout.CountryAmount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.EmployeeAmount")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.EmployeeAmount")).value, out double val))
                    Helpers.DefaultValue.PPKPayout.EmployeeAmount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.EmployerAmount")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.EmployerAmount")).value, out double val))
                    Helpers.DefaultValue.PPKPayout.EmployerAmount = val;
            }
            if (_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.Percentage")) != null)
            {
                if (double.TryParse(_configuration.FirstOrDefault(a => a.name.Equals("PPKPayout.Percentage")).value, out double val))
                    Helpers.DefaultValue.PPKPayout.Percentage = val;
            }

        }

        public void InsertConfiguration(Configuration configuration)
        {
            _dbcontext.configuration.Add(configuration);
            _dbcontext.SaveChanges();
            RefreshConfiguration();
        }

        public void DeleteConfiguration(Configuration configuration)
        {
            _dbcontext.configuration.Remove(configuration);
            _dbcontext.SaveChanges();
            RefreshConfiguration();
        }

        public bool UpdateConfiguration(Configuration configuration)
        {
            var configurationUpdate = _dbcontext.configuration.FirstOrDefault(a => a.iid == configuration.iid);
            if (configurationUpdate != null)
            {
                configurationUpdate.name = configuration.name;
                configurationUpdate.value = configuration.value;
                _dbcontext.SaveChanges();
                RefreshConfiguration();

                return true;
            }
            return false;
        }
    }
}
