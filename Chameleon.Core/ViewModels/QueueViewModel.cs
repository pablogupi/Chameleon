﻿using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Chameleon.Services.Services;
using MediaManager;
using MediaManager.Library;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Chameleon.Core.ViewModels
{
    public class QueueViewModel : BaseViewModel
    {
        private readonly IUserDialogs _userDialogs;
        private readonly IMediaManager _mediaManager;

        public QueueViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService,
                                IUserDialogs userDialogs, IMediaManager mediaManager, IPlaylistService playlistService)
            : base(logProvider, navigationService)
        {
            _userDialogs = userDialogs ?? throw new ArgumentNullException(nameof(userDialogs));
            _mediaManager = mediaManager ?? throw new ArgumentNullException(nameof(mediaManager));
        }

        public MvxObservableCollection<IMediaItem> MediaItems { get; set; } = new MvxObservableCollection<IMediaItem>();

        private IMediaItem _selectedMediaItem;
        public IMediaItem SelectedMediaItem
        {
            get => _selectedMediaItem;
            set => SetProperty(ref _selectedMediaItem, value);
        }

        public string QueueTitle => $"{GetText("Queue")} ({MediaItems.Count})";

        private IMvxAsyncCommand<IMediaItem> _playerCommand;
        public IMvxAsyncCommand<IMediaItem> PlayerCommand => _playerCommand ?? (_playerCommand = new MvxAsyncCommand<IMediaItem>(Play));

        private IMvxAsyncCommand _closeCommand;
        public IMvxAsyncCommand CloseCommand => _closeCommand ?? (_closeCommand = new MvxAsyncCommand(() => Application.Current.MainPage.Navigation.PopModalAsync()));

        private async Task Play(IMediaItem mediaItem)
        {
            await _mediaManager.PlayQueueItem(mediaItem);
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();
            MediaItems.ReplaceWith(_mediaManager.MediaQueue);
            RaisePropertyChanged(nameof(QueueTitle));
        }
    }
}
