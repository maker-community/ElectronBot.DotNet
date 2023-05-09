using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.ViewModels;
using Windows.Storage;

namespace ElectronBot.Braincase.Services;

public class UserDataService
{
    private const string _userSettingsKey = "IdentityUser";

    private UserViewModel _user;

    private readonly IdentityService _identityService;

    private readonly IMicrosoftGraphService _microsoftGraphService;

    public event EventHandler<UserViewModel> UserDataUpdated;

    public UserDataService(IdentityService identityService, IMicrosoftGraphService microsoftGraphService)
    {
        _identityService = identityService;
        _microsoftGraphService = microsoftGraphService;
    }

    public void Initialize()
    {
        _identityService.LoggedIn += OnLoggedIn;
        _identityService.LoggedOut += OnLoggedOut;
    }

    public async Task<UserViewModel> GetUserAsync()
    {
        if (_user == null)
        {
            _user = await GetUserFromCacheAsync();
            if (_user == null)
            {
                _user = GetDefaultUserData();
            }
        }

        return _user;
    }

    private async void OnLoggedIn(object sender, EventArgs e)
    {
        _user = await GetUserFromGraphApiAsync();
        UserDataUpdated?.Invoke(this, _user);
    }

    private async void OnLoggedOut(object sender, EventArgs e)
    {
        _user = null;
        await ApplicationData.Current.LocalFolder.SaveAsync<LocalUser>(_userSettingsKey, null);
    }

    private async Task<UserViewModel> GetUserFromCacheAsync()
    {
        var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<LocalUser>(_userSettingsKey);
        return await GetUserViewModelFromData(cacheData);
    }

    private async Task<UserViewModel> GetUserFromGraphApiAsync()
    {
        var accessToken = await _identityService.GetAccessTokenForGraphAsync();
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        LocalUser localUser = null;

        var userData = await _microsoftGraphService.GetUserInfoAsync();
        if (userData != null)
        {
            localUser = new LocalUser
            {
                BusinessPhones = userData.BusinessPhones.ToList(),
                DisplayName = userData.DisplayName,
                GivenName = userData.GivenName,
                Id = userData.Id,
                JobTitle = userData.JobTitle,
                Mail = userData.Mail,
                MobilePhone = userData.MobilePhone,
                OfficeLocation = userData.OfficeLocation,
                PreferredLanguage = userData.PreferredLanguage,
                Surname = userData.Surname,
                UserPrincipalName = userData.UserPrincipalName

            };
            localUser.Photo = await _microsoftGraphService.GetUserPhotoAsync();
            await ApplicationData.Current.LocalFolder.SaveAsync(_userSettingsKey, localUser);
        }

        return await GetUserViewModelFromData(localUser);
    }

    private async Task<UserViewModel> GetUserViewModelFromData(LocalUser userData)
    {
        if (userData == null)
        {
            return null;
        }

        var userPhoto = string.IsNullOrEmpty(userData.Photo)
            ? ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
            : await ImageHelper.ImageFromStringAsync(userData.Photo);

        return new UserViewModel()
        {
            Name = userData.DisplayName,
            UserPrincipalName = userData.UserPrincipalName,
            Photo = userPhoto
        };
    }

    private UserViewModel GetDefaultUserData()
    {
        return new UserViewModel()
        {
            Name = _identityService.GetAccountUserName(),
            Photo = ImageHelper.ImageFromAssetsFile("DefaultIcon.png")
        };
    }
}
