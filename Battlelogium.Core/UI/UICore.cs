﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Battlelogium.Core.UI
{
    /// <summary>
    /// Provides methods to interact with the User Interface
    /// </summary>
    public class UICore
    {

        Origin managedOrigin;
        UIWindow mainWindow;
        Config config;
        Battlelog battlelog;

        public UICore(UIWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.battlelog = this.mainWindow.battlelog;
            this.config = this.mainWindow.config;
            this.mainWindow.mainGrid.Children.Add(battlelog.battlelogWebview);
            this.mainWindow.Title = "Battlelogium - " + battlelog.battlefieldName;
            this.mainWindow.Closed += mainWindow_Closed;
            this.mainWindow.PreviewKeyDown += mainWindow_PreviewKeyDown;
            this.battlelog.battlelogWebview.PropertyChanged += battlelogWebview_IsLoading;

            this.managedOrigin = new Origin();

            if (config.ManageOrigin)
            {
                this.managedOrigin.StartOrigin();
            }

            switch (this.config.WindowedMode)
            {
                case true:
                    this.mainWindow.SetWindowed();
                    break;
                case false:
                    this.mainWindow.SetFullScreen();
                    break;
            }

            if (config.UseSoftwareRender)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
        }

        private void mainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                this.battlelog.battlelogWebview.Reload();
                return;
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Enter)
            {
                switch (this.mainWindow.IsFullscreen)
                {
                    case true:
                        this.mainWindow.SetWindowed();
                        break;
                    case false:
                        this.mainWindow.SetFullScreen();
                        break;
                }
                e.Handled = true;
            }
        }
        
        private void mainWindow_Closed(object sender, EventArgs e)
        {
            if (config.ManageOrigin)
            {
                this.managedOrigin.KillOrigin(config.WaitTimeToKillOrigin * 1000);
            }
        }
        private void battlelogWebview_IsLoading(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsLoading"))
            {
                if (this.battlelog.battlelogWebview.IsLoading)
                {
                    this.mainWindow.Dispatcher.Invoke(new Action(delegate { this.mainWindow.loadingIcon.Visibility = Visibility.Visible; }));

                }
                else
                {
                    this.mainWindow.Dispatcher.Invoke(new Action(delegate { this.mainWindow.loadingIcon.Visibility = Visibility.Collapsed; }));
                }

            }
        }
    }
}
