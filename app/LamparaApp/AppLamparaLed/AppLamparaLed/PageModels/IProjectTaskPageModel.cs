using AppLamparaLed.Models;
using CommunityToolkit.Mvvm.Input;

namespace AppLamparaLed.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}