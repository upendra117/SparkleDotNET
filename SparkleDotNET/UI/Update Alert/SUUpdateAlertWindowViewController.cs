﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using KNFoundation;
using KNFoundation.KNKVC;

namespace SparkleDotNET {
    class SUUpdateAlertWindowViewController : KNViewController, KNKVOObserver {

        private SUHost host;
        private SUAppcastItem item;
        private KNViewController actionViewController;
        private Canvas currentActionContainer;

        public SUUpdateAlertWindowViewController(UserControl view)
            : base(view) {


                this.AddObserverToKeyPathWithOptions(this, "Host", 0, null);
                this.AddObserverToKeyPathWithOptions(this, "Item", 0, null);
        }

        

        public SUHost Host {
            get { return host; }
            set {
                this.WillChangeValueForKey("Host");
                host = value;
                this.DidChangeValueForKey("Host");
            }
        }

        public SUAppcastItem Item {
            get { return item; }
            set {
                this.WillChangeValueForKey("Item");
                item = value;
                this.DidChangeValueForKey("Item");
            }
        }

        public void ObserveValueForKeyPathOfObject(string keyPath, object obj, Dictionary<string, object> change, object context) {
            if (keyPath.Equals("Host") || keyPath.Equals("Item")) {
                // Set up the UI!

                if (Host != null && Item != null) {

                    ReleaseNotes.Source = Item.ReleaseNotesURL;
                    UpdateHeaderLabel.Text = String.Format("A new version of {0} is available!", Host.Name);

                    if (Item.DisplayVersionString.Equals(Host.DisplayVersion)) {
                        // Display more info if the version strings are the same; useful for betas.
                        UpdateHeaderDescription.Text = String.Format("{0} {1} ({3}) is available - you have {2} ({4}). Would you like to download it now?",
                            Host.Name, Item.DisplayVersionString, Host.DisplayVersion, Item.VersionString, Host.Version);
                    } else {
                        UpdateHeaderDescription.Text = String.Format("{0} {1} is available - you have {2}. Would you like to download it now?",
                            Host.Name, Item.DisplayVersionString, Host.DisplayVersion);
                    }
                    IconView.Source = Host.Icon;
                }
            }
        }

        public KNViewController ActionViewController {
            get { return actionViewController; }
            set {

                if (!Object.ReferenceEquals(value, actionViewController)) {

                    this.WillChangeValueForKey("ActionViewController");

                    if (actionViewController != null) {
                        CurrentActionContainer.Children.Remove(actionViewController.View);
                    }

                    actionViewController = value;

                    if (value != null) {

                        Canvas.SetTop(value.View, 0);
                        Canvas.SetLeft(value.View, 0);

                        value.View.Width = CurrentActionContainer.ActualWidth;
                        value.View.Height = CurrentActionContainer.ActualHeight;

                        CurrentActionContainer.Children.Add(value.View);

                    }

                    this.DidChangeValueForKey("ActionViewController");
                }
            }
        }

        private void ActionContainerResized(object sender, SizeChangedEventArgs e) {
            if (ActionViewController != null) {
                ActionViewController.View.Width = e.NewSize.Width;
                ActionViewController.View.Height = e.NewSize.Height;
            }
        }

   

        public Image IconView {
            protected set;
            get;
        }

        public TextBlock UpdateHeaderLabel {
            protected set;
            get;
        }

        public TextBlock UpdateHeaderDescription {
            protected set;
            get;
        }

        public WebBrowser ReleaseNotes {
            protected set;
            get;
        }

        public Canvas CurrentActionContainer {
            protected set {
                value.SizeChanged += ActionContainerResized;
                currentActionContainer = value;
            }
            get {
                return currentActionContainer;
            }
        }
   
    }
}