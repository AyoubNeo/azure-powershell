// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.Sql.Auditing.Model;
using Microsoft.Azure.Commands.Sql.Auditing.Services;
using Microsoft.Azure.Commands.Sql.Common;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Sql.Auditing.Cmdlet
{
    /// <summary>
    /// The base class for all SQL server auditing Management Cmdlets
    /// </summary>
    public abstract class SqlDatabaseServerAuditingCmdletBase : AzureSqlCmdletBase<AuditingPolicyModel, SqlAuditAdapter>
    {
        /// <summary>
        /// Gets or sets the name of the database server to use.
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "SQL Database server name.")]
        [ResourceNameCompleter("Microsoft.Sql/servers", "ResourceGroupName")]
        [ValidateNotNullOrEmpty]
        public string ServerName { get; set; }

        public virtual AuditType AuditType { get; set; }

        /// <summary>
        /// Provides the model element that this cmdlet operates on
        /// </summary>
        /// <returns>A model object</returns>
        protected override AuditingPolicyModel GetEntity()
        {
            if (AuditType == AuditType.NotSet)
            {
                AuditType = AuditType.Blob;
                var blobPolicy = GetEntityHelper();

                // If the user has blob auditing on on the resource we return that policy no matter what is his table auditing policy
                if ((blobPolicy != null) && (blobPolicy.AuditState == AuditStateType.Enabled))
                {
                    return blobPolicy;
                }

                // The user don't have blob auditing policy on
                AuditType = AuditType.Table;
                var tablePolicy = GetEntityHelper();
                return tablePolicy;
            }

            // The user has selected specific audit type
            var policy = GetEntityHelper();
            return policy;
        }

        /// <summary>
        /// Creation and initialization of the ModelAdapter object
        /// </summary>
        /// <returns>An initialized and ready to use ModelAdapter object</returns>
        protected override SqlAuditAdapter InitModelAdapter()
        {
            return new SqlAuditAdapter(DefaultProfile.DefaultContext);
        }

        /// <summary>
        /// This method is responsible to call the right API in the communication layer that will eventually send the information in the 
        /// object to the REST endpoint
        /// </summary>
        /// <param name="baseModel">The model object with the data to be sent to the REST endpoints</param>
        protected override AuditingPolicyModel PersistChanges(AuditingPolicyModel baseModel)
        {
            if (AuditType == AuditType.Table)
            {
                ModelAdapter.SetServerAuditingPolicy(baseModel as ServerAuditingPolicyModel, 
                    DefaultContext.Environment.GetEndpoint(AzureEnvironment.Endpoint.StorageEndpointSuffix));
            }
            if (AuditType == AuditType.Blob)
            {
                ModelAdapter.SetServerAuditingPolicy(baseModel as ServerBlobAuditingPolicyModel,
                    DefaultContext.Environment.GetEndpoint(AzureEnvironment.Endpoint.StorageEndpointSuffix));
            }
            return null;
        }

        private AuditingPolicyModel GetEntityHelper()
        {
            if (AuditType == AuditType.Table)
            {
                ServerAuditingPolicyModel model;
                ModelAdapter.GetServerAuditingPolicy(ResourceGroupName, ServerName, out model);
                return model;
            }

            if (AuditType == AuditType.Blob)
            {
                ServerBlobAuditingPolicyModel blobModel = new ServerBlobAuditingPolicyModel
                {
                    AuditType = AuditType.Blob
                };
                ModelAdapter.GetServerBlobAuditingPolicy(ResourceGroupName, ServerName, blobModel);
                return blobModel;
            }

            return null;
        }
    }
}