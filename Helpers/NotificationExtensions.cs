using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Notifications;
using Models;
using System;
using System.Collections.Generic;
using Windows.UI.Notifications;

namespace Helpers
{
    /// <summary>
    /// NotificationExtensions class
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Method which sends an update to the live tile.
        /// </summary>
        /// <param name="profile">The content of the tile</param>
        public static void SendTileNotification(Profile profile)
        {
            try
            {
                TileContent tileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        DisplayName = "Area clienti iliad",
                        Branding = TileBranding.Name,

                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "Chiamate",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "effettuate",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.VoiceTime,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "Traffico",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "dati",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.DataUsed,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                    }
                                }
                            }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                TextStacking = TileTextStacking.Center,
                                Children =
                            {
                                new AdaptiveGroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "Chiamate",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "effettuate",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.VoiceTime,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "SMS",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "inviati",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.SMSCount,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "MMS",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "inviati",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.MMSCount,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup()
                                        {
                                            HintWeight = 1,
                                            Children =
                                            {
                                                new AdaptiveText()
                                                {
                                                    Text = "Traffico",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = "dati",
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                },
                                                new AdaptiveText()
                                                {
                                                    Text = profile.Local.DataUsed,
                                                    HintAlign = AdaptiveTextAlign.Center,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                }
                                            }
                                        },
                                    }
                                }
                            }
                            }
                        },
                    }
                };

                var tileNotification = new TileNotification(tileContent.GetXml());

                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent(ex.Message, new Dictionary<string, string> { { "exception", ex.ToString() } });
            }
        }
    }
}
