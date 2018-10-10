using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.EDS.Core.Dispatch;
using Sitecore.EDS.Core.Reporting;
using Sitecore.EDS.Providers.SparkPost.Configuration;
using Sitecore.EDS.Providers.SparkPost.Dispatch;
using Sitecore.EDS.Providers.SparkPost.Services;
using Newtonsoft.Json.Linq;
using Sitecore.StringExtensions;

namespace Sitecore.Support.EDS.Providers.SparkPost.Dispatch
{
  public class DispatchProvider : Sitecore.EDS.Providers.SparkPost.Dispatch.DispatchProvider
  {
    private readonly IConfigurationStore _configurationStore;
    public DispatchProvider(ConnectionPoolManager connectionPoolManager, IEnvironmentId environmentIdentifier, IConfigurationStore configurationStore, string returnPath) : base(connectionPoolManager, environmentIdentifier, configurationStore, returnPath)
    {
      this._configurationStore = configurationStore;
    }

    protected override void SetMessageHeaders(EmailMessage message)
    {
      SparkPostClientCredentials credentials = this._configurationStore.GetCredentials(false);
      JObject jObject = JObject.FromObject(new
      {
        options = new Dictionary<string, object>
    {
      {
        "open_tracking",
        (object)credentials.OpenTracking
      },
      {
        "click_tracking",
        (object)credentials.ClickTracking
      },
      {
        "ip_pool",
        (object)(credentials.IpPool ?? "sp_shared")
      }
    },
        metadata = new Dictionary<string, string>
    {
          {
        "contact_id",
        message.ContactIdentifier
      },
      {
        "contact_identifier",
        message.ContactIdentifier
      },
      {
        "message_id",
        message.MessageId
      },
      {
        "instance_id",
        message.MessageId
      },
      {
        "campaign_id",
        message.CampaignId
      },
      {
        "target_language",
        message.TargetLanguage
      },
      {
        "test_value_index",
        message.TestValueIndex.HasValue ? message.TestValueIndex.Value.ToString() : string.Empty
      },
      {
        "email_address_history_entry_id",
        message.EmailAddressHistoryEntryId.ToString()
      }
    }
      });
      message.Headers.Set("X-MSYS-API", jObject.ToString());
      if (!message.ReplyTo.IsNullOrEmpty())
      {
        message.Headers.Set("Reply-To", message.ReplyTo);
      }
    }
  }
}