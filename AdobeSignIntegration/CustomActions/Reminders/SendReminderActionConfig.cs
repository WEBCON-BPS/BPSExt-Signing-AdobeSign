﻿using WebCon.BpsExt.Signing.AdobeSign.CustomActions.Models.Configuration;
using WebCon.WorkFlow.SDK.Common;
using WebCon.WorkFlow.SDK.ConfigAttributes;

namespace WebCon.BpsExt.Signing.AdobeSign.CustomActions.Reminders
{
    public class SendReminderActionConfig : PluginConfiguration
    {
        [ConfigGroupBox(DisplayName = "AdobeSign API Settings")]
        public ApiConfiguration ApiConfig { get; set; }   

        [ConfigGroupBox(DisplayName = "Recipients selection")]
        public ItemListConfig Users { get; set; }
    }

    public class ApiConfiguration : BaseConfiguration
    {
        [ConfigEditableFormFieldID(DisplayName = "Document ID", Description = "Select the text field where the Document ID was saved.")]
        public int OperationFildId { get; set; }

        [ConfigEditableFormFieldID(DisplayName = "Status", Description = "Select the text field where the Document status was saved.")]
        public int StatusFildId { get; set; }
    }

    public class ItemListConfig
    {
        [ConfigEditableItemList(DisplayName = "Signers Item List")]
        public SignersList SignersList { get; set; }
    }

    public class SignersList : IConfigEditableItemList
    {
        public int ItemListId { get; set; }     

        [ConfigEditableItemListColumnID(DisplayName = "E-mail", IsRequired = true)]
        public int SignerMailColumnID { get; set; }      
    }
}