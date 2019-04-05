﻿using System;
using System.Collections.Generic;
using Datory;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;

namespace SiteServer.CMS.Apis
{
    public class ContentApi : IContentApi
    {
        private ContentApi() { }

        private static ContentApi _instance;
        public static ContentApi Instance => _instance ?? (_instance = new ContentApi());

        public IContentInfo GetContentInfo(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            return ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
        }

        public List<IContentInfo> GetContentInfoList(int siteId, int channelId, string whereString, string orderString, int limit, int offset)
        {
            if (siteId <= 0 || channelId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelId);

            var list = DataProvider.ContentRepository.GetContentInfoList(tableName, whereString, orderString, offset, limit);
            var retVal = new List<IContentInfo>();
            foreach (var contentInfo in list)
            {
                retVal.Add(contentInfo);
            }
            return retVal;
        }

        public int GetCount(int siteId, int channelId, string whereString)
        {
            if (siteId <= 0 || channelId <= 0) return 0;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelId);

            return DataProvider.ContentRepository.GetCount(tableName, whereString);
        }

        public string GetTableName(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return string.Empty;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            return ChannelManager.GetTableName(siteInfo, nodeInfo);
        }

        public List<TableColumn> GetTableColumns(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var tableStyleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, nodeInfo);
            var tableColumnList = new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = ContentAttribute.Title,
                    DataType = DataType.VarChar
                }
            };

            foreach (var styleInfo in tableStyleInfoList)
            {
                tableColumnList.Add(new TableColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DataType = DataType.VarChar
                });
            }

            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsTop,
                DataType = DataType.VarChar
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsRecommend,
                DataType = DataType.VarChar
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsHot,
                DataType = DataType.VarChar
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.IsColor,
                DataType = DataType.VarChar
            });
            tableColumnList.Add(new TableColumn
            {
                AttributeName = ContentAttribute.AddDate,
                DataType = DataType.DateTime
            });

            return tableColumnList;
        }

        public string GetContentValue(int siteId, int channelId, int contentId, string attributeName)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;
            
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            return channelInfo.ContentRepository.GetValue<string>(contentId, attributeName);
        }

        public IContentInfo NewInstance(int siteId, int channelId)
        {
            return new ContentInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                AddDate = DateTime.Now
            };
        }

        //public void SetValuesToContentInfo(int siteId, int channelId, NameValueCollection form, IContentInfo contentInfo)
        //{
        //    var siteInfo = SiteManager.GetSiteInfo(siteId);
        //    var nodeInfo = NodeManager.GetChannelInfo(siteId, channelId);
        //    var tableName = NodeManager.GetTableName(siteInfo, nodeInfo);
        //    var tableStyle = NodeManager.GetTableStyle(siteInfo, nodeInfo);
        //    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(siteId, channelId);

        //    var extendImageUrl = ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl);
        //    if (form.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
        //    {
        //        form[extendImageUrl] = form[StringUtils.LowerFirst(extendImageUrl)];
        //    }

        //    InputTypeParser.AddValuesToAttributes(tableStyle, tableName, siteInfo, relatedIdentities, form, contentInfo.ToNameValueCollection(), ContentAttribute.HiddenAttributes);
        //}

        public int Insert(int siteId, int channelId, IContentInfo contentInfo)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            return DataProvider.ContentRepository.Insert(tableName, siteInfo, channelInfo, (ContentInfo)contentInfo);
        }

        public void Update(int siteId, int channelId, IContentInfo contentInfo)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            DataProvider.ContentRepository.Update(siteInfo, channelInfo, (ContentInfo)contentInfo);
        }

        public void Delete(int siteId, int channelId, int contentId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var contentIdList = new List<int> { contentId };
            channelInfo.ContentRepository.UpdateTrashContents(siteId, channelId, contentIdList);
        }

        public IList<int> GetContentIdList(int siteId, int channelId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            return channelInfo.ContentRepository.GetContentIdListCheckedByChannelId(siteId, channelId);
        }

        public string GetContentUrl(int siteId, int channelId, int contentId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetContentUrl(siteInfo, ChannelManager.GetChannelInfo(siteId, channelId), contentId, false);
        }
    }
}
