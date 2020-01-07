using HtmlAgilityPack;
using Project_Alphonse_Elric.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Resources;

namespace Helpers
{
    /// <summary>
    /// ClientExtensions class
    /// </summary>
    public class ClientExtensions
    {
        private ResourceLoader Resources = new ResourceLoader();

        public Profile AccountDetails { get; set; } = new Profile();

        private HttpFormUrlEncodedContent _POSTData;

        private HttpClient _client = new HttpClient();

        /// <summary>
        /// Method which connects to the Iliad personal area of the user and then
        /// reads the HTML code of the web page to get the details about costs 
        /// and consumes.
        /// </summary>
        /// <param name="username">The username of the account</param>
        /// <param name="password">The password of the account</param>
        /// <returns></returns>
        public async Task Authenticate(string username, string password)
        {
            _client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            await _client.GetAsync(new Uri(Resources.GetString("LoginURL")));

            _POSTData = new HttpFormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login-ident", username),
                new KeyValuePair<string, string>("login-pwd", password)
            });

            HttpResponseMessage response = await _client.PostAsync(new Uri(Resources.GetString("LoginURL")), _POSTData);
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            ReadConsumes(document);
        }

        /// <summary>
        /// Method which logout the current user.
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("LogoutURL")));
            response.EnsureSuccessStatusCode();
        }

        public async Task SendEnableRequest(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(url + "1"));
            response.EnsureSuccessStatusCode();
        }

        public async Task SendDisableRequest(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(url + "0"));
            response.EnsureSuccessStatusCode();
        }

        #region Consumes

        /// <summary>
        /// Method which connects to user's consumes page and then reads the HTML
        /// code of the web page to get the details about costs and consumes.
        /// </summary>
        /// <returns></returns>
        public async Task GetConsumes()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("ConsumesURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            ReadConsumes(document);
        }

        /// <summary>
        /// Method which reads the HTML code of the user consumes page to get details about
        /// costs and consumes.
        /// </summary>
        /// <param name="document">The HTML document where to get information</param>
        private void ReadConsumes(HtmlDocument document)
        {
            HtmlNode[] subnode = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Equals("current-user__infos")).ToArray()[0].Descendants().ToArray();

            AccountDetails.Name = subnode[2].InnerText.Trim();
            AccountDetails.ID = subnode[5].InnerText.Trim();
            AccountDetails.PhoneNumber = subnode[6].InnerText.Trim();

            subnode = document.DocumentNode.Descendants().Where(tag => tag.GetAttributeValue("class", "").Contains("red")).ToArray();
            AccountDetails.Local.VoiceTime = subnode[2].InnerText;
            AccountDetails.Local.VoiceExtra = subnode[3].InnerText;
            AccountDetails.Local.SMSCount = subnode[4].InnerText;
            AccountDetails.Local.SMSExtra = subnode[5].InnerText;
            AccountDetails.Local.DataUsed = subnode[6].InnerText;
            AccountDetails.Local.DataExtra = subnode[7].InnerText;
            AccountDetails.Local.MMSCount = subnode[10].InnerText;
            AccountDetails.Local.MMSExtra = subnode[11].InnerText;

            AccountDetails.Roaming.VoiceTime = subnode[12].InnerText;
            AccountDetails.Roaming.VoiceExtra = subnode[13].InnerText;
            AccountDetails.Roaming.SMSCount = subnode[14].InnerText;
            AccountDetails.Roaming.SMSExtra = subnode[15].InnerText;
            AccountDetails.Roaming.DataUsed = subnode[16].InnerText;
            AccountDetails.Roaming.DataExtra = subnode[17].InnerText;
            AccountDetails.Roaming.MMSCount = subnode[21].InnerText;
            AccountDetails.Roaming.MMSExtra = subnode[22].InnerText;

            AccountDetails.RemainingCredit = subnode[0].InnerText;

            AccountDetails.NextRenewal = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Equals("end_offerta")).ToArray()[0].InnerText.Trim().Substring(49);
        }

        #endregion

        #region Options

        /// <summary>
        /// Method which connects to user's options page and then reads the HTML
        /// code of the web page to get the activation status of the available options.
        /// </summary>
        /// <returns></returns>
        public async Task GetOptions()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("OptionsURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.PublishPhoneNumber = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.MarketingAgreement = node[4].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;

            response = await _client.GetAsync(new Uri(Resources.GetString("NumberOptionsURL")));
            response.EnsureSuccessStatusCode();

            document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.PaidNumbers = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.PaidBankNumbers = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;

            response = await _client.GetAsync(new Uri(Resources.GetString("DataCeilingOptionsURL")));
            response.EnsureSuccessStatusCode();

            document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.UnlockLocal = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.UnlockItaly = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.UnlockRoaming = node[4].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
        }

        #endregion

        #region Services

        /// <summary>
        /// Method which connects to user's services page and then reads the HTML
        /// code of the web page to get the activation status of the available services.
        /// </summary>
        /// <returns></returns>
        public async Task GetServices()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("ServicesURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveServices.BlockUnknown = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveServices.TransferToVoicemail = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveServices.TransferProtection = node[4].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveServices.UserNotAvailable = node[6].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveServices.FastCalls = node[8].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveServices.Filter = node[10].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;

            if (AccountDetails.ActiveServices.BlockUnknown)
            {
                await CheckRedirectToVoicemailUnknown();
            }
        }

        #region Trasferire le chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate

        /// <summary>
        /// Method which connects to user's services page and then reads the HTML
        /// code of the web page to get the activation status of Trasferire le 
        /// chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate
        /// service.
        /// </summary>
        /// <returns></returns>
        private async Task CheckRedirectToVoicemailUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("AnonymousServiceURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] subnode = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveServices.RedirectToVoicemailUnknown = subnode[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
        }

        #endregion

        #region Chiamate rapide

        /// <summary>
        /// Method which adds a phone number to fast calls.
        /// </summary>
        /// <param name="name">The name of the fast call</param>
        /// <param name="target">The target number of the fast call</param>
        /// <param name="shortTarget">The abbreviation of the target number</param>
        /// <returns></returns>
        public async Task AddFastCall(string name, string target, string shortTarget)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format(Resources.GetString("AddFastCallURL"), WebUtility.UrlEncode(name), WebUtility.UrlEncode(target), shortTarget)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which removes a phone number from fast calls.
        /// </summary>
        /// <param name="uri">The uri related to the fast call</param>
        /// <returns></returns>
        public async Task DeleteFastCall(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format(Resources.GetString("DeleteFastCallURL"), uri)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which connects to user's fast calls page and then reads the HTML
        /// code of the web page to get defined fast calls.
        /// </summary>
        /// <returns></returns>
        public async Task GetFastCalls()
        {
            AccountDetails.ActiveServices.FastCallList.Clear();

            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("FastCallsURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] inputSubnode = document.DocumentNode.Descendants("input").Where(tag => tag.GetAttributeValue("disabled", "").Equals("disabled")).ToArray();

            HtmlNode[] aSubnode = document.DocumentNode.Descendants("a").Where(tag => tag.GetAttributeValue("data-action", "").Equals("delete")).ToArray();

            for (int i = 0, j = 0; i < inputSubnode.Length; j++)
            {
                AccountDetails.ActiveServices.FastCallList.Add(new FastCall(inputSubnode[i++].GetAttributeValue("value", ""), WebUtility.HtmlDecode(inputSubnode[i++].GetAttributeValue("value", "")), inputSubnode[i++].GetAttributeValue("value", ""), aSubnode[j].GetAttributeValue("href", "")));
            }
        }

        #endregion

        #endregion

        #region Voicemail

        /// <summary>
        /// Method which connects to user's voicemail page and then reads the HTML
        /// code of the web page to get the details about voicemail messages and 
        /// options.
        /// </summary>
        /// <returns></returns>
        public async Task GetMessages()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("VoicemailURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            AccountDetails.Voicemail.MessagesList.Clear();

            HtmlNode[] node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Equals("messages-list")).ToArray()[0].Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-l msg")).ToArray();

            foreach (HtmlNode subnode in node)
            {
                string dateTime = System.Text.RegularExpressions.Regex.Replace(subnode.ChildNodes[1].ChildNodes[2].InnerText.Split('(')[0].Replace(" a ", " alle ").Trim(), @"\s+", " ");
                AccountDetails.Voicemail.MessagesList.Add(new Message(subnode.ChildNodes[1].ChildNodes[1].InnerText.Trim(), dateTime, subnode.ChildNodes[3].ChildNodes[1].Attributes[1].Value, subnode.ChildNodes[5].ChildNodes[1].ChildNodes[1].Attributes[0].Value, subnode.ChildNodes[5].ChildNodes[1].ChildNodes[1].Attributes[1].Value));
            }

            node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.Voicemail.ShowCallerID = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.Voicemail.ShowTimeDate = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.Voicemail.ProtectVoicemail = node[4].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.Voicemail.PersonalizedAdvert = node[6].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
        }

        /// <summary>
        /// Method which removes a voicemail message.
        /// </summary>
        /// <param name="ID">The ID of the message</param>
        /// <returns></returns>
        public async Task DeleteMessage(string ID)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format(Resources.GetString("DeleteVoicemailURL"), ID)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which downloads a voicemail message.
        /// </summary>
        /// <param name="message">The voicemail message to download</param>
        /// <returns></returns>
        public async Task<StorageFile> DownloadMessage(Message message)
        {
            StorageFile file = await DownloadsFolder.CreateFileAsync(string.Format("{0} {1}.wav", message.Sender, message.DateTime.Replace(':', '-')), CreationCollisionOption.GenerateUniqueName);

            IBuffer buffer = await _client.GetBufferAsync(message.Source);
            await FileIO.WriteBufferAsync(file, buffer);

            return file;
        }

        #region Notifiche

        /// <summary>
        /// Method which connects to user's voicemail page and then reads the HTML
        /// code of the web page to get defined notifications.
        /// </summary>
        /// <returns></returns>
        public async Task GetNotifications()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(Resources.GetString("VoicemailURL")));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml(await response.Content.ReadAsStringAsync());

            AccountDetails.Voicemail.VoicemailNotificationList.Clear();

            HtmlNode[] modeNodes = document.DocumentNode.Descendants("option").Where(tag => tag.GetAttributeValue("selected", "").Equals("selected")).ToArray();

            HtmlNode[] mailNodes = document.DocumentNode.Descendants("input").Where(tag => tag.GetAttributeValue("class", "").Equals("mdc-text-field__input")).ToArray();

            for(int i = 0; i < modeNodes.Length; i++)
            {
                AccountDetails.Voicemail.VoicemailNotificationList.Add(new VoicemailNotification(modeNodes[i].InnerText.Trim(), mailNodes[i].GetAttributeValue("value", "").Trim()));
            }
        }

        /// <summary>
        /// Method which defines a new notication.
        /// </summary>
        /// <param name="mode">The mode of the notification</param>
        /// <param name="mail">The mail where to send the notification</param>
        /// <returns></returns>
        public async Task AddNotification(string mode, string mail)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format(Resources.GetString("AddNotificationURL"), mail, mode)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which removes a notification.
        /// </summary>
        /// <param name="mail">The mail where to send the notification</param>
        /// <returns></returns>
        public async Task DeleteNotification(string mail)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format(Resources.GetString("DeleteNotificationURL"), mail)));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #endregion
    }
}