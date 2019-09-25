﻿// ----------------------------------------------------------------------------------
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
// ---------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.Common.Storage.ResourceModel
{
    using Microsoft.WindowsAzure.Commands.Common.Storage.ResourceModel;
    using Microsoft.Azure.Cosmos.Table;
    using System;
    using Microsoft.WindowsAzure.Commands.Common.Attributes;

    /// <summary>
    /// Azure storage table object
    /// </summary>
    public class AzureStorageTable : AzureStorageBase
    {
        /// <summary>
        /// Cloud table object
        /// </summary>
        [Ps1Xml(Label = "Table End Point", Target = ViewControl.Table, GroupByThis = true, ScriptBlock = "$_.CloudTable.ServiceClient.BaseUri")]
        [Ps1Xml(Label = "Name", Target = ViewControl.Table, ScriptBlock = "$_.Name", Position = 0)]
        public CloudTable CloudTable { get; private set; }

        /// <summary>
        /// Table uri
        /// </summary>
        [Ps1Xml(Label = "Uri", Target = ViewControl.Table, ScriptBlock = "$_.Uri", Position = 1)]
        public Uri Uri { get; private set; }

        /// <summary>
        /// Azure storage table constructor
        /// </summary>
        /// <param name="table">Cloud table object</param>
        public AzureStorageTable(CloudTable table)
        {
            Name = table.Name;
            CloudTable = table;
            Uri = table.Uri;
        }
    }
}