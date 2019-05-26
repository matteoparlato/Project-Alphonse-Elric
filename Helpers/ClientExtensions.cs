using HtmlAgilityPack;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Helpers
{
    /// <summary>
    /// ClientExtensions class
    /// </summary>
    public static class ClientExtensions
    {
        public static Profile AccountDetails { get; set; } = new Profile();

        private static HttpFormUrlEncodedContent _POSTData;

        private static HttpClient _client = new HttpClient();

        /// <summary>
        /// Method which connects to the Iliad personal area of the user and then
        /// reads the HTML code of the web page to get the details about costs 
        /// and consumes.
        /// </summary>
        /// <param name="username">The username of the account</param>
        /// <param name="password">The password of the account</param>
        /// <returns></returns>
        public static async Task Authenticate(string username, string password)
        {
            _client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            await _client.GetAsync(new Uri("http://www.iliad.it/account/"));

            _POSTData = new HttpFormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login-ident", username),
                new KeyValuePair<string, string>("login-pwd", password)
            });

            HttpResponseMessage response = await _client.PostAsync(new Uri("https://www.iliad.it/account/"), _POSTData);
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            ReadConsumes(document);
        }

        /// <summary>
        /// Method which logout the current user.
        /// </summary>
        /// <returns></returns>
        public static async Task Logout()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/?logout=user"));
            response.EnsureSuccessStatusCode();
        }

        #region Consumes

        /// <summary>
        /// Method which connects to user's consumes page and then reads the HTML
        /// code of the web page to get the details about costs and consumes.
        /// </summary>
        /// <returns></returns>
        public static async Task GetConsumes()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/consumi-e-credito"));
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
        private static void ReadConsumes(HtmlDocument document)
        {
            HtmlNode[] subnode = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Equals("current-user")).ToArray()[0].Descendants().ToArray();

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
        public static async Task GetOptions()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni"));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.PublishPhoneNumber = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;

            response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/blocco-numeri-a-pagamento"));
            response.EnsureSuccessStatusCode();

            document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.PaidNumbers = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.PaidBankNumbers = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;

            response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling"));
            response.EnsureSuccessStatusCode();

            document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            node = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveOptions.UnlockLocal = node[0].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.UnlockItaly = node[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
            AccountDetails.ActiveOptions.UnlockRoaming = node[4].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
        }

        #region Pubblicazione in elenco

        /// <summary>
        /// Method which enables Pubblicazione in elenco option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnablePublishPhoneNumber()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni?update=elenco&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Pubblicazione in elenco option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisablePublishPhoneNumber()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni?update=elenco&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Blocca le chiamate verso i numeri a pagamento e gli SMS+

        /// <summary>
        /// Method which enables Blocca le chiamate verso i numeri a pagamento e gli SMS+ option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnablePaidNumbers()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/blocco-numeri-a-pagamento?premium=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Blocca le chiamate verso i numeri a pagamento e gli SMS+ option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisablePaidNumbers()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/blocco-numeri-a-pagamento?premium=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Blocca gli SMS bancari a pagamento

        /// <summary>
        /// Method which enables Blocca gli SMS bancari a pagamento option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnablePaidBankNumbers()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/blocco-numeri-a-pagamento?bank=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Blocca gli SMS bancari a pagamento option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisablePaidBankNumbers()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/blocco-numeri-a-pagamento?bank=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Sblocco connessione dati oltre il limite incluso nel forfait

        /// <summary>
        /// Method which enables Sblocco connessione dati oltre il limite incluso nel forfait option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableUnlockLocal()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?fairuse=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Sblocco connessione dati oltre il limite incluso nel forfait option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableUnlockLocal()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?fairuse=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Sblocco connessione dati in Italia oltre 50€ fatturati

        /// <summary>
        /// Method which enables Sblocco connessione dati in Italia oltre 50€ fatturati option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableUnlockItaly()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?billing=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Sblocco connessione dati in Italia oltre 50€ fatturati option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableUnlockItaly()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?billing=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Sblocco consumo dei dati in roaming (Europa e resto del mondo) oltre i 50€

        /// <summary>
        /// Method which enables Sblocco consumo dei dati in roaming (Europa e resto del mondo) oltre i 50€ option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableUnlockRoaming()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?roaming=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Sblocco consumo dei dati in roaming (Europa e resto del mondo) oltre i 50€ option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableUnlockRoaming()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/le-mie-opzioni/data-ceiling?roaming=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #endregion

        #region Services

        /// <summary>
        /// Method which connects to user's services page and then reads the HTML
        /// code of the web page to get the activation status of the available services.
        /// </summary>
        /// <returns></returns>
        public static async Task GetServices()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi"));
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

        #region Blocco numeri nascosti

        /// <summary>
        /// Method which enables Blocco numeri nascosti option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableBlockUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/block_anonymous?activate=1"));
            response.EnsureSuccessStatusCode();

            await CheckRedirectToVoicemailUnknown();
        }

        /// <summary>
        /// Method which disables Blocco numeri nascosti option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableBlockUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/block_anonymous?activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Trasferire le chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate

        /// <summary>
        /// Method which enables Trasferire le chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableRedirectToVoicemailUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/block_anonymous?activate=1&forward=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Trasferire le chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableRedirectToVoicemailUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/block_anonymous?activate=1&forward=0"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which connects to user's services page and then reads the HTML
        /// code of the web page to get the activation status of Trasferire le 
        /// chiamate anonime alla segreteria telefonica anziché rifiutare le chiamate
        /// service.
        /// </summary>
        /// <returns></returns>
        private static async Task CheckRedirectToVoicemailUnknown()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/block_anonymous"));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml((await response.Content.ReadAsStringAsync()));

            HtmlNode[] subnode = document.DocumentNode.Descendants("div").Where(tag => tag.GetAttributeValue("class", "").Contains("grid-c w-2 w-desktop-2 w-tablet-4 as__cell as__status")).ToArray();

            AccountDetails.ActiveServices.RedirectToVoicemailUnknown = subnode[2].GetAttributeValue("class", "").Contains("as__status--active") ? true : false;
        }

        #endregion

        #region Inoltro verso segreteria telefonica all'estero

        /// <summary>
        /// Method which enables Inoltro verso segreteria telefonica all'estero option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableTransferToVoicemail()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/voicemail_roaming?activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Inoltro verso segreteria telefonica all'estero option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableTransferToVoicemail()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/voicemail_roaming?activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Protezione contro il trasferimento di chiamate

        /// <summary>
        /// Method which enables Protezione contro il trasferimento di chiamate option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableTransferProtection()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=block_redirect&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Protezione contro il trasferimento di chiamate option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableTransferProtection()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=block_redirect&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Utente non disponibile

        /// <summary>
        /// Method which enables Utente non disponibile option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableUserNotAvailable()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=absent_subscriber&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Utente non disponibile option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableUserNotAvailable()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=absent_subscriber&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Chiamate rapide

        /// <summary>
        /// Method which enables Chiamate rapide option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableFastCalls()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=speed_dial&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Chiamate rapide option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableFastCalls()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=speed_dial&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which adds a phone number to fast calls.
        /// </summary>
        /// <param name="name">The name of the fast call</param>
        /// <param name="target">The target number of the fast call</param>
        /// <param name="short">The abbreviation of the target number</param>
        /// <returns></returns>
        public static async Task AddFastCall(string name, string target, string @short)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format("https://www.iliad.it/account/i-miei-servizi/numeros_abreges?action=add&name={0}&target={1}&short={2}", WebUtility.UrlEncode(name), WebUtility.UrlEncode(target), @short)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which removes a phone number from fast calls.
        /// </summary>
        /// <param name="uri">The uri related to the fast call</param>
        /// <returns></returns>
        public static async Task DeleteFastCall(string uri)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format("https://www.iliad.it{0}&action=delete", uri)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which connects to user's fast calls page and then reads the HTML
        /// code of the web page to get defined fast calls.
        /// </summary>
        /// <returns></returns>
        public static async Task GetFastCalls()
        {
            AccountDetails.ActiveServices.FastCallList.Clear();

            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi/numeros_abreges"));
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

        #region Filtro Chiamate & SMS/MMS

        /// <summary>
        /// Method which enables Filtro Chiamate & SMS/MMS option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableFilter()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=filter_rules&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Filtro Chiamate & SMS/MMS option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableFilter()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/i-miei-servizi?update=filter_rules&activate=0"));
            response.EnsureSuccessStatusCode();
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
        public static async Task GetMessages()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica"));
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
        public static async Task DeleteMessage(string ID)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format("https://www.iliad.it/account/segreteria-telefonica/messaggio_vocale?id={0}&action=delete", ID)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which downloads a voicemail message.
        /// </summary>
        /// <param name="message">The voicemail message to download</param>
        /// <returns></returns>
        public static async Task<StorageFile> DownloadMessage(Message message)
        {
            StorageFile file = await DownloadsFolder.CreateFileAsync(string.Format("{0} {1}.wav", message.Sender, message.DateTime.Replace(':', '-')), CreationCollisionOption.GenerateUniqueName);

            IBuffer buffer = await _client.GetBufferAsync(message.Source);
            await FileIO.WriteBufferAsync(file, buffer);

            return file;
        }

        #region Visualizza il numero del chiamante

        /// <summary>
        /// Method which enables Visualizza il numero del chiamante  option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableShowCallerID()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica?update=0&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Visualizza il numero del chiamante  option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableShowCallerID()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica?update=0&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Visualizza per ogni messaggio la data e l'orario della chiamata

        /// <summary>
        /// Method which enables Visualizza per ogni messaggio la data e l'orario della chiamata option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnableShowTimeDate()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica?update=1&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Visualizza per ogni messaggio la data e l'orario della chiamata option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisableShowTimeDate()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica?update=1&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Personalizza l'annuncio della tua segreteria telefonica

        /// <summary>
        /// Method which enables Personalizza l'annuncio della tua segreteria telefonica option.
        /// </summary>
        /// <returns></returns>
        public static async Task EnablePersonalizedAdvert()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica/Annuncio?update=3&activate=1"));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which disables Personalizza l'annuncio della tua segreteria telefonica option.
        /// </summary>
        /// <returns></returns>
        public static async Task DisablePersonalizedAdvert()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica/Annuncio?update=3&activate=0"));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Notifiche

        /// <summary>
        /// Method which connects to user's voicemail page and then reads the HTML
        /// code of the web page to get defined notifications.
        /// </summary>
        /// <returns></returns>
        public static async Task GetNotifications()
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri("https://www.iliad.it/account/segreteria-telefonica"));
            response.EnsureSuccessStatusCode();

            HtmlDocument document = new HtmlDocument() { OptionFixNestedTags = true };

            document.LoadHtml(await response.Content.ReadAsStringAsync());

            AccountDetails.Voicemail.VoicemeilNotificationList.Clear();

            HtmlNode[] modeNodes = document.DocumentNode.Descendants("option").Where(tag => tag.GetAttributeValue("selected", "").Equals("selected")).ToArray();

            HtmlNode[] mailNodes = document.DocumentNode.Descendants("input").Where(tag => tag.GetAttributeValue("class", "").Equals("mdc-text-field__input")).ToArray();

            for(int i = 0; i < modeNodes.Length; i++)
            {
                AccountDetails.Voicemail.VoicemeilNotificationList.Add(new VoicemailNotification(modeNodes[i].InnerText.Trim(), mailNodes[i].GetAttributeValue("value", "").Trim()));
            }
        }

        /// <summary>
        /// Method which defines a new notication.
        /// </summary>
        /// <param name="mode">The mode of the notification</param>
        /// <param name="mail">The mail where to send the notification</param>
        /// <returns></returns>
        public static async Task AddNotification(string mode, string mail)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format("https://www.iliad.it/account/segreteria-telefonica/notifiche?email={0}&action=add&type={1}", mail, mode)));
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Method which removes a notification.
        /// </summary>
        /// <param name="mail">The mail where to send the notification</param>
        /// <returns></returns>
        public static async Task DeleteNotification(string mail)
        {
            HttpResponseMessage response = await _client.GetAsync(new Uri(string.Format("https://www.iliad.it/account/segreteria-telefonica/notifiche?email={0}&action=delete", mail)));
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #endregion
    }
}