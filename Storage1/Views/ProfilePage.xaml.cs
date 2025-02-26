using Storage1.Models;
using Storage1.Services;
using System;
using System.IO;
using Microsoft.Maui.Controls;
using System.Text.Json;

namespace Storage1.Views
{
    public partial class ProfilePage : ContentPage
    {
        private string filePath;
        private Profile _currentProfile; 
        private readonly DatabaseService _dbService = new(); 

        public ProfilePage()
        {
            InitializeComponent();
            filePath = Path.Combine(FileSystem.AppDataDirectory, "profile.json");
            LoadProfile();
        }

        private void LoadProfile()
        {
            // Checks if the profile file exists
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                // Initializing _currentProfile
                _currentProfile = JsonSerializer.Deserialize<Profile>(json); 

                NameEntry.Text = _currentProfile.Name;
                SurnameEntry.Text = _currentProfile.Surname;
                EmailEntry.Text = _currentProfile.Email;
                BioEntry.Text = _currentProfile.Bio;

                if (!string.IsNullOrEmpty(_currentProfile.ProfilePicture))
                {
                    ProfileImage.Source = ImageSource.FromFile(_currentProfile.ProfilePicture);
                }
            }
            else
            {
                _currentProfile = new Profile(); // Initialize with a new profile if no file exists
            }
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            // Validation that all fields are filled.
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(SurnameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(BioEntry.Text) ||
                ProfileImage.Source == null)
            {
                // Displays an error message if any field is empty
                StatusLabel.Text = "Please fill in all fields and choose a profile picture.";
                StatusLabel.TextColor = Colors.Red;
                return;
            }

            // Update _currentProfile with the new data
            _currentProfile.Name = NameEntry.Text;
            _currentProfile.Surname = SurnameEntry.Text;
            _currentProfile.Email = EmailEntry.Text;
            _currentProfile.Bio = BioEntry.Text;
            _currentProfile.ProfilePicture = ProfileImage.Source?.ToString();

            // Save the profile to the database
            SaveProfileToDatabase(_currentProfile);

            // Save the profile to JSON (optional, if you still want to use JSON)
            string json = JsonSerializer.Serialize(_currentProfile);
            File.WriteAllText(filePath, json);

            StatusLabel.Text = "Profile saved successfully!";
            StatusLabel.TextColor = Colors.Green;

            ClearForm();
        }

        private async void SaveProfileToDatabase(Profile profile)
        {
            await _dbService.UpdateProfileAsync(profile); // Save to SQLite database
        }

        private async void OnChoosePictureClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                string newFilePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                using (var stream = await result.OpenReadAsync())
                {
                    using (var newStream = File.Create(newFilePath))
                    {
                        await stream.CopyToAsync(newStream);
                    }
                }
                ProfileImage.Source = ImageSource.FromFile(newFilePath);
            }
        }

        private void ClearForm()
        {
            // Clear all entry fields
            NameEntry.Text = string.Empty;
            SurnameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            BioEntry.Text = string.Empty;
            ProfileImage.Source = null;
        }

        private void OnViewProfileClicked(object sender, EventArgs e)
        {
            if (_currentProfile != null)
            {
                string profileInfo = $"Name: {_currentProfile.Name}\n" +
                                     $"Surname: {_currentProfile.Surname}\n" +
                                     $"Email: {_currentProfile.Email}\n" +
                                     $"Bio: {_currentProfile.Bio}";

                DisplayAlert("Profile Information", profileInfo, "OK");
            }
            else
            {
                DisplayAlert("Error", "No profile information available", "OK");
            }
        }
    }
}