﻿//*********************************************************
// Copyright (c) Microsoft Corporation
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the ""License""); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS 
// OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR 
// PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
//
// See the Apache Version 2.0 License for specific language 
// governing permissions and limitations under the License.
//*********************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MixERP.Net.VCards;

namespace OneNoteServiceSamplesWinUniversal.OneNoteApi.Pages
{
	/// <summary>
	/// Class to show a selection of examples for getting page meta data or content via HTTP GET to the OneNote API
	/// - Getting a page info is represented via the GET HTTP verb.
	/// - Getting meta data for ALL pages under ALL notebooks/sections the user has is represented by the Uri: https://www.onenote.com/api/v1.0/pages
	///    - Getting meta data for pages under ALL notebooks/sections but with filters applied is represented by the Uri: https://www.onenote.com/api/v1.0/pages?$filter=...
    /// - Searching for text content in pages under ALL notebooks/sections is represented by the Uri: https://www.onenote.com/api/v1.0/pages?search={searchterm} 
	/// - Getting meta data for ALL pages under a given section is represented by the Uri: https://www.onenote.com/api/v1.0/sections/{sectionId}/pages
	/// - Getting meta data for a specific page is represented by the Uri: https://www.onenote.com/api/v1.0/pages/{pageId}
	/// - Getting access to the content (aka data) in a given page is represented by the Uri: https://www.onenote.com/api/v1.0/pages/{pageId}/content
    /// For more info, see http://dev.onenote.com/docs
	/// </summary>
	/// <remarks>
	/// NOTE: Our APIs support OData-style querying so you can use query params like $filter, $orderby, $top, $skip, $select etc.
	/// For more info, see http://www.odata.org/getting-started/basic-tutorial/
	/// NOTE: It is not the goal of this code sample to produce well re-factored, elegant code.
	/// You may notice code blocks being duplicated in various places in this project.
	/// We have deliberately added these code blocks to allow anyone browsing the sample
	/// to easily view all related functionality in near proximity.
	/// </remarks>
	/// <code>
	///  var client = new HttpClient();
	///  client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	///  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Auth.GetAuthToken());
	///  var getMessage = new HttpRequestMessage(HttpMethod.Get, "https://www.onenote.com/api/v1.0/pages");
	///  HttpResponseMessage response = await client.SendAsync(getMessage);
	/// </code>
	public static class GetPagesExample
	{
		#region Examples of GET https://www.onenote.com/api/v1.0/pages AND query params

		/// <summary>
		/// Get meta data for ALL pages under ALL of the user's notebooks/sections
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <returns>The converted HTTP response message</returns>
		public static async Task<List<ApiBaseResponse>> GetAllPages(bool debug, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute + "pages");

			HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}

		/// <summary>
		/// Get meta data by skipping the first X pages and for the Top Y pages under ALL of the user's notebooks/sections
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="skipCount">the number of first 1..n pages to skip</param>
		/// <param name="topCount">the total number of pages to return after the skipCount</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <returns>The converted HTTP response message</returns>
		/// <example>https://www.onenote.com/api/v1.0/pages?$skip=20&$top=10
		/// returns pages 21 to 30 (by skipping the first 20 pages). Default page count returned is 100.</example>
		public static async Task<List<ApiBaseResponse>> GetAllPagesWithSkipAndTopQueryParams(bool debug, int skipCount, int topCount, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get, String.Format(apiRoute + "pages?$skip={0}&$top={1}", skipCount, topCount));

			HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}

		/// <summary>
		/// Get meta data for ALL pages matching a filter criteria under ALL of the user's notebooks/sections.
		/// In this example the filter criteria is searching for a substring in the page titles.
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="titleFilterString">substring to search for in page titles</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <returns>The converted HTTP response message</returns>
		/// <example>https://www.onenote.com/api/v1.0/pages?$filter=contains(title,'API')
		///  returns all pages with title containing the case-sensitive substring 'API'</example>
		public static async Task<List<ApiBaseResponse>> GetAllPagesWithTitleContainsFilterQueryParams(bool debug, string titleFilterString, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get,
				String.Format(apiRoute + "pages?$filter=contains(title,'{0}')", WebUtility.UrlEncode(titleFilterString)));

            HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}

		/// <summary>
		/// Get selected meta data for ALL pages and ordered in a given fashion.
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="orderByFieldName">the field that is used to order the results</param>
		/// <param name="selectFieldNames">meta data fields to return in the response</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <returns>The converted HTTP response message</returns>
		/// <example>https://www.onenote.com/api/v1.0/pages?$select=id,title&$orderby=title%20asc
		/// returns only the id and title meta data values sorting the result alphabetically by title in ascending order</example>
		public static async Task<List<ApiBaseResponse>> GetAllPagesWithOrderByAndSelectQueryParams(bool debug, string orderByFieldName, string selectFieldNames, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get,
				String.Format(apiRoute + "pages?$select={0}&$orderby={1}",
					WebUtility.UrlEncode(selectFieldNames),
					WebUtility.UrlEncode(orderByFieldName)));

			HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}

		#endregion

		#region Examples of Search GET https://www.onenote.com/api/v1.0/pages?search={searchTerm}

		/// <summary>
		/// Search ALL pages containing the (full-text) search term under ALL of the user's notebooks/sections.
		/// In this example the search criteria is searching for a substring in the page's content including title.
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="searchTerm">string to search for</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <returns>The converted HTTP response message</returns>
		/// <example>https://www.onenote.com/api/v1.0/pages?search=cat
		///  returns all pages containing the case-insensitive string 'cat'</example>
		public static async Task<List<ApiBaseResponse>> SearchAllPages(bool debug, string searchTerm, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get,
				String.Format(apiRoute + "pages?search={0}", WebUtility.UrlEncode(searchTerm)));

			HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}
		#endregion

		#region Example of GET https://www.onenote.com/api/v1.0/sections/{sectionId}/pages

		/// <summary>
		/// Get meta data for ALL pages under a given section
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="sectionId">Id of the section for which the page are returned</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <remarks>  The sectionId can be fetched by querying the user's sections (e.g. GET https://www.onenote.com/api/v1.0/sections ).
		/// NOTE: Using this approach, you can still query pages with ALL the different params shown in examples above.
		/// </remarks>
		/// <returns>The converted HTTP response message</returns>
		public static async Task<List<ApiBaseResponse>> GetAllPagesUnderASpecificSectionId(bool debug, string sectionId, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute + "sections/" + sectionId + "/pages");

            HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslateListOfPagesResponse(response);
		}

		#endregion

		#region Example of GET https://www.onenote.com/api/v1.0/pages/{pageId}

		/// <summary>
		/// Get meta data for a specific page
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="pageId">Id of the page for which the meta data is returned</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <remarks>  The pageId can be fetched from an earlier GET/POST response of pages endpoint (e.g. GET https://www.onenote.com/api/v1.0/pages ).
		/// NOTE: Using this approach, you can still query pages with ALL the different params shown in examples above.
		/// </remarks>
		/// <returns>The converted HTTP response message</returns>
		public static async Task<ApiBaseResponse> GetASpecificPageMetadata(bool debug, string pageId, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: API only supports JSON response.
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

			// Prepare an HTTP GET request to the Pages endpoint
			var getMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute + "pages/" + pageId);

            HttpResponseMessage response = await client.SendAsync(getMessage);

			return await TranslatePageResponse(response);
		}

		#endregion

		#region Example of Page content recall: GET https://www.onenote.com/api/v1.0/pages/{pageId}/content 

		/// <summary>
		/// Get the content for a specific page
		/// Previously example showed us how to get page meta data. This example shows how to get the content of the page.
		/// </summary>
		/// <param name="debug">Run the code under the debugger</param>
		/// <param name="pageId">Id of the page for which the content is returned</param>
		/// <param name="provider"></param>
		/// <param name="apiRoute"></param>
		/// <remarks>  The pageId can be fetched from an earlier GET/POST response of pages endpoint (e.g. GET https://www.onenote.com/api/v1.0/pages ).
		/// </remarks>
		/// <returns>The converted HTTP response message</returns>
		public static async Task<ApiBaseResponse> GetASpecificPageContent(bool debug, string pageId, AuthProvider provider, string apiRoute)
		{
			if (debug)
			{
				Debugger.Launch();
				Debugger.Break();
			}

			var client = new HttpClient();

			// Note: Page content is returned in a same HTML fashion supported by POST Pages
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

			// Not adding the Authentication header would produce an unauthorized call and the API will return a 401
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
				await Auth.GetAuthToken(provider));

            // Prepare an HTTP GET request to the Pages endpoint
            var getMessage = new HttpRequestMessage(HttpMethod.Get, apiRoute + "pages/" + pageId + "/content"); // ?includeIDs=true");

			HttpResponseMessage response = await client.SendAsync(getMessage);

            ApiBaseResponse apiResponse = await TranslatePageContentResponse(response);

            // check if response has a vcard if yes then process vcard. 
            bool isvCardPreset = apiResponse.Body.Contains(".vcf");

            if (isvCardPreset)
            {
                // load it into xml dom or somehow extract the <object> tag holding the .vcf entry. 
                var getvCard = new HttpRequestMessage(HttpMethod.Get, "https://www.onenote.com/api/v1.0/me/notes/resources/1-20864c3a4d904265b609203706ab868c!1-6198c848-9990-4de3-a235-ff07a2901b67/$value");
                HttpResponseMessage vCardResponse = await client.SendAsync(getvCard);
                string vCardBody = await vCardResponse.Content.ReadAsStringAsync();

                Debug.WriteLine("vCardBody:" + vCardBody);
                var bizCards = Deserializer.GetVCards(vCardBody);
                foreach (var card in bizCards)
                {
                    var topBizCard = card;
                    Debug.WriteLine("Formatted Name: " + topBizCard.FormattedName);
                    Debug.WriteLine("First Name: " + topBizCard.FirstName);
                    Debug.WriteLine("Last Name: " + topBizCard.LastName);
                    Debug.WriteLine("Org Name: " + topBizCard.Organization);
                    Debug.WriteLine("Url: " + topBizCard.Url.ToString());
                    foreach (var email in topBizCard.Emails)
                    {
                        Debug.WriteLine("EMail Type (" + email.Type + ") - Tel Number : " + email.EmailAddress);
                    }
                    foreach (var telNo in topBizCard.Telephones)
                    {
                        Debug.WriteLine("Tel Type (" + telNo.Type + ") - Tel Number : " + telNo.Number);                        
                    }
                }

            }

            return apiResponse;
		}

		#endregion

		#region Helper methods used in the examples

		/// <summary>
		/// Convert the HTTP response message into a simple structure suitable for apps to process
		/// </summary>
		/// <param name="response">The response to convert</param>
		/// <returns>A simple response</returns>
		private static async Task<List<ApiBaseResponse>> TranslateListOfPagesResponse(HttpResponseMessage response)
		{
			var apiBaseResponse = new List<ApiBaseResponse>();
			string body = await response.Content.ReadAsStringAsync();
			if (response.StatusCode == HttpStatusCode.OK
				/* GET Pages calls always return 200-OK upon success */)
			{
					var content = JObject.Parse(body);
					apiBaseResponse = new List<ApiBaseResponse>(JsonConvert.DeserializeObject<List<PageResponse>>(content["value"].ToString()));
			}
			if (apiBaseResponse.Count == 0)
			{
				apiBaseResponse.Add(new ApiBaseResponse());
			}

			// Extract the correlation id.  Apps should log this if they want to collect the data to diagnose failures with Microsoft support 
			IEnumerable<string> correlationValues;
			if (response.Headers.TryGetValues("X-CorrelationId", out correlationValues))
			{
				apiBaseResponse[0].CorrelationId = correlationValues.FirstOrDefault();
			}
			apiBaseResponse[0].StatusCode = response.StatusCode;
			apiBaseResponse[0].Body = body;
			return apiBaseResponse;
		}


		/// <summary>
		/// Convert the HTTP response message into a simple structure suitable for apps to process
		/// </summary>
		/// <param name="response">The response to convert</param>
		/// <returns>A simple response</returns>
		private static async Task<ApiBaseResponse> TranslatePageResponse(HttpResponseMessage response)
		{
			ApiBaseResponse apiBaseResponse;
			string body = await response.Content.ReadAsStringAsync();
			if (response.StatusCode == HttpStatusCode.OK
				/* GET Pages calls always return 200-OK upon success */)
			{
				apiBaseResponse = JsonConvert.DeserializeObject<PageResponse>(body);
			}
			else
			{
				apiBaseResponse = new ApiBaseResponse();
			}

			// Extract the correlation id.  Apps should log this if they want to collect the data to diagnose failures with Microsoft support 
			IEnumerable<string> correlationValues;
			if (response.Headers.TryGetValues("X-CorrelationId", out correlationValues))
			{
				apiBaseResponse.CorrelationId = correlationValues.FirstOrDefault();
			}
			apiBaseResponse.StatusCode = response.StatusCode;
			apiBaseResponse.Body = body;
			return apiBaseResponse;
		}

		/// <summary>
		/// Convert the HTTP response message into a simple structure suitable for apps to process
		/// </summary>
		/// <param name="response">The response to convert</param>
		/// <returns>A simple response</returns>
		private static async Task<ApiBaseResponse> TranslatePageContentResponse(HttpResponseMessage response)
		{
			string body = await response.Content.ReadAsStringAsync();
			var apiBaseResponse = new ApiBaseResponse();

			// Extract the correlation id.  Apps should log this if they want to collect the data to diagnose failures with Microsoft support 
			IEnumerable<string> correlationValues;
			if (response.Headers.TryGetValues("X-CorrelationId", out correlationValues))
			{
				apiBaseResponse.CorrelationId = correlationValues.FirstOrDefault();
			}
			apiBaseResponse.StatusCode = response.StatusCode;
			apiBaseResponse.Body = body;
			return apiBaseResponse;
		}

		#endregion
	}
}
