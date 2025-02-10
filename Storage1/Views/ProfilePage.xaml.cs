using System.Text.Json;
using Storage1.Models;

namespace Storage1.Views;

public partial class ProfilePage : ContentPage
{
	
    private string filePath;

    public ProfilePage()
	{
		InitializeComponent();
        filePath = Path.Combine(FileSystem.AppDataDirectory, "profile.json");
        LoadProfile();
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

        Profile profile = new Profile
        {
            Name = NameEntry.Text,
            Surname = SurnameEntry.Text,
            Email = EmailEntry.Text,
            Bio = BioEntry.Text,
            ProfilePicture = ProfileImage.Source?.ToString()
        };

        string json = JsonSerializer.Serialize(profile);
        File.WriteAllText(filePath, json);
        StatusLabel.Text = "Profile saved successfully!";
        StatusLabel.TextColor = Colors.Green;

        ClearForm();
    }

    private void LoadProfile()
    {
        // Check if the profile file exists
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Profile profile = JsonSerializer.Deserialize<Profile>(json);

            NameEntry.Text = profile.Name;
            SurnameEntry.Text = profile.Surname;
            EmailEntry.Text = profile.Email;
            BioEntry.Text = profile.Bio;

            if (!string.IsNullOrEmpty(profile.ProfilePicture))
            {
                ProfileImage.Source = ImageSource.FromFile(profile.ProfilePicture);
            }
        }
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
}
