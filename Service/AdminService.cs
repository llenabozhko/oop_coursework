using System.Collections.Generic;
using System.Linq;
using oop_coursework.Models;

namespace oop_coursework.Services
{
    public class AdminService
    {
        private readonly DataService _dataService;

        public AdminService(DataService dataService)
        {
            _dataService = dataService;
        }

        public List<User> GetAllUsers(string? roleFilter = null)
        {
            var users = _dataService.GetStudents()
                .Cast<User>()
                .Concat(_dataService.GetTeachers())
                .Concat(_dataService.GetAdministrators());

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            return users.ToList();
        }

        public bool CanDeleteUser(User user, Administrator admin)
        {
            if (user is Administrator && _dataService.GetAdministrators().Count <= 1)
            {
                return false;
            }

            if (user.Id == admin.Id)
            {
                return false;
            }

            return true;
        }

        public void DeleteUser(int userId)
        {
            _dataService.DeleteUser(userId);
        }
    }
}
