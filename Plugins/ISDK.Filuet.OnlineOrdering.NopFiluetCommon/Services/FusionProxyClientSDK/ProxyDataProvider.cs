﻿using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FusionProxyClientSDK;
using System; using System.Collections.Concurrent; using System.Collections.Generic; using System.Linq; 
namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK {
    //TODO Настройки надо зарефакторить и вынести
    public static class ProxyDataProvider     {
        #region Methods          private static readonly List<Location> _locations = new List<Location>         {             new Location()             {                 Name = "LATVIA. Pickup",                 WarehouseNumber = "LR",                 ProcessingLocation = "LR",                 OrgId = 294,                 OrderTypeId = "2991",                 FreightCodes = new List<string>() {"PU", "PU1"},             },             new Location()             {                 Name = "LATVIA. ShipTo",                 WarehouseNumber = "LV",                 ProcessingLocation = "LV",                 OrgId = 294,                 OrderTypeId = "2991",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },             new Location()             {                 Name = "LITHUANIA. Ship TO",                 WarehouseNumber = "LV",                 ProcessingLocation = "LT",                 OrgId = 294,                 OrderTypeId = "2997",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },             new Location()             {                 Name = "ESTONIA. Ship To",                 WarehouseNumber = "LV",                 ProcessingLocation = "EE",                 OrgId = 294,                 OrderTypeId = "3023",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },             new Location()             {                 Name = "LATVIA. Pickup",                 WarehouseNumber = "LR",                 ProcessingLocation = "IL",                 OrgId = 294,                 OrderTypeId = "2991",                 FreightCodes = new List<string>() {"PU"},             },             new Location()             {                 Name = "LATVIA. ShipTo",                 WarehouseNumber = "LV",                 ProcessingLocation = "IL",                 OrgId = 294,                 OrderTypeId = "2991",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },             new Location()             {                 Name = "LITHUANIA. Ship TO",                 WarehouseNumber = "LT",                 ProcessingLocation = "IT",                 OrgId = 294,                 OrderTypeId = "2997",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },             new Location()             {                 Name = "ESTONIA. Ship To",                 WarehouseNumber = "EE",                 ProcessingLocation = "IE",                 OrgId = 294,                 OrderTypeId = "3023",                 FreightCodes = new List<string>() {"BLC", "BLD", "BLO", "BLX", "NOF", "BLH", "OTH"},             },               new Location()             {                 Name = "INDIA",                 WarehouseNumber = "W5",                 ProcessingLocation = "W5",                 OrgId = 253,                 OrderTypeId = "2803",                 FreightCodes = new List<string>() {"PU"},             },             new Location()             {                 Name = "VIETNAM",                 WarehouseNumber = "VO",                 ProcessingLocation = "VN",                 OrgId = 1589,                 OrderTypeId = "4310",                 FreightCodes = new List<string>() {"PU"},             },             new Location()             {                 Name = "KV Regular Sales Order-KAZ",                 WarehouseNumber = "KZ",                 ProcessingLocation = "KN",                 OrgId = 2357,                 OrderTypeId = "6515",                 FreightCodes = new List<string>() {"PU"},             },             new Location()             {                 Name = "KV Regular Sales Order-KAZ",                 WarehouseNumber = "KZ",                 ProcessingLocation = "KV",                 OrgId = 2357,                 OrderTypeId = "6515",                 FreightCodes = new List<string>() {"PU"},             },             new Location()             {                 Name = "Cyprus",                 WarehouseNumber = "U7",                 ProcessingLocation = "CG",                 OrgId = 292,                 OrderTypeId = "2755",                 FreightCodes = new List<string>() {"PU","C2P","C2D"},             }         };           public static string GetWarehouseNumber(string processingLocation, string freightCode)         {             return _locations.FirstOrDefault(s => s.ProcessingLocation.Equals(processingLocation) &&                                                   s.FreightCodes.Contains(freightCode))?.WarehouseNumber;         }          public static int GetOrgId(string processingLocation)         {             return _locations.FirstOrDefault(s => s.ProcessingLocation.Equals(processingLocation))?.OrgId ?? 0;         }          public static string GetOrderTypeId(string processingLocation)         {             return _locations.FirstOrDefault(s => s.ProcessingLocation.Equals(processingLocation))?.OrderTypeId;         }          private static readonly ConcurrentDictionary<string, UserToken> _tokensByUser =             new ConcurrentDictionary<string, UserToken>();          private static readonly ConcurrentDictionary<string, UserToken> _tokens =             new ConcurrentDictionary<string, UserToken>();          private static UserToken CreateToken(string userId)         {             return new UserToken(userId, TimeSpan.FromMinutes(60));         }          public static string GenerateToken(string userId)         {             var token = _tokensByUser.GetOrAdd(userId, CreateToken);              if (!_tokens.ContainsKey(token.Token))             {                 _tokens[token.Token] = token;             }              if (token.Expired)             {                 var newToken = CreateToken(userId);                 _tokensByUser[userId] = newToken;                 _tokens[newToken.Token] = newToken;                 _tokens.TryRemove(token.Token, out token);                  token = newToken;             }              return token.Token;         }           public static bool ValidateToken(string token)         {             return _tokens.ContainsKey(token) && !_tokens[token].Expired;         }
        #endregion     } }