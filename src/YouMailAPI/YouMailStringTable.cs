/*************************************************************************************************
 * Copyright (c) 2018 Gilles Khouzam
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software withou
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/*************************************************************************************************/

using System;
namespace MagikInfo.YouMailAPI
{
    public class YMST
    {
#if DEVSERVER
        public const string c_loginProtocol = "http://";
        public const string c_unsecureProtocol = "http://";
        public const string c_secureProtocol = "http://";

        public const string c_server = "api.dev.youmail.com";
        public const string c_debugServer = c_server;
#else
        public const string c_loginProtocol = "https://";
        public const string c_unsecureProtocol = "http://";
        public const string c_secureProtocol = "https://";
        public const string c_server = "api.youmail.com";
#if DEBUG
        public const string c_debugServer = "apidebug.youmail.com";
#else
        private const string c_debugServer = c_server;
#endif
#endif
        public const string c_pollServer = "apipoll.youmail.com";
        public const string c_protocolToken = "://";

        public const string c_authToken = "authToken";
        public const string c_authorization = "Authorization";
        public const string c_disableCodes = "disableCodes";
        public const string c_phoneForwardingInstruction = "phoneForwardingInstruction";
        public const string c_enableCodes = "enableCodes";
        public const string c_enableMessage = "enableMessage";
        public const string c_disableMessage = "disableMessage";
        public const string c_forwardingNotRequired = "forwardingNotRequired";
        public const string c_accessPoint = "accessPoint";
        public const string c_conferenceNumber = "conferenceNumber";
        public const string c_defaultConferenceNumber = "defaultConferenceNumber";
        public const string c_carrierId = "carrierId";
        public const string c_carrierName = "carrierName";
        public const string c_carrierHint = "carrierHint";
        public const string c_activatingCodePrimary = "activatingCodePrimary";
        public const string c_activatingCodeSecondary = "activatingCodeSecondary";
        public const string c_deactivatingCode = "deactivatingCode";
        public const string c_carrierSupportsForwarding = "carrierSupportsForwarding";
        public const string c_carrierManagesForwarding = "carrierManagesForwarding";
        public const string c_carrierHasForwardingCharges = "carrierHasForwardingCharges";
        public const string c_carrierOffersPrepaid = "carrierOffersPrepaid";
        public const string c_carrierSupportsPrepaidForwarding = "carrierSupportsPrepaidForwarding";
        public const string c_carrierOnlineAccountRequired = "carrierOnlineAccountRequired";
        public const string c_callingCarrierSupportRequired = "callingCarrierSupportRequired";
        public const string c_didNumberRequired = "didNumberRequired";
        public const string c_localNumberRequired = "localNumberRequired";
        public const string c_dropboxOnly = "dropboxOnly";
        public const string c_carrierForwardingUrl = "carrierForwardingUrl";
        public const string c_carrierSupportNumber = "carrierSupportNumber";
        public const string c_forwardingInstructions = "forwardingInstructions";
        public const string c_deactivatingInstructions = "deactivatingInstructions";
        public const string c_specialInstructions = "specialInstructions";
        public const string c_notes = "notes";
        public const string c_forwardingNumber = "forwardingNumber";
        public const string c_retrievalNumber = "retrievalNumber";
        public const string c_anyNotesToShow = "anyNotesToShow";
        public const string c_forwardToNumber = "forwardToNumber";
        public const string c_pickupNumber = "pickupNumber";
        public const string c_primaryPhoneNumber = "primaryPhoneNumber";
        public const string c_value = "value";
        public const string c_id = "id";
        public const string c_ids = "ids";
        public const string c_data = "data";
        public const string c_dataUrl = "dataUrl";
        public const string c_name = "name";
        public const string c_ackEntryCount = "ackEntryCount";
        public const string c_newEntryCount = "newEntryCount";
        public const string c_visibleEntryCount = "visibleEntryCount";
        public const string c_folder = "folder";
        public const string c_folders = "folders";
        public const string c_entries = "entries";
        public const string c_entry = "entry";

        public const string c_long = "long";
        public const string c_setGreeting = "<greeting><id>{0}</id></greeting>";
        public const string c_description = "description";
        public const string c_duration = "duration";
        public const string c_greeting = "greeting";
        public const string c_greetings = "greetings";
        public const string c_recommendation = "recommendation";
        public const string c_mobileProductUrl = "mobileProductUrl";
        public const string c_pitch = "pitch";
        public const string c_product = "product";
        public const string c_productName = "productName";
        public const string c_productSku = "productSku";
        public const string c_recommendationType = "recommendationType";
        public const string c_unitPrice = "unitPrice";
        public const string c_webProductUrl = "webProductUrl";
        public const string c_YouMail = "YouMail "; // Don't forget the space here
        public const string c_ecommerceStatus = "ecommerceStatus";
        public const string c_order = "order";
        public const string c_trial = "trial";
        public const string c_orderType = "orderType";
        public const string c_status = "status";
        public const string c_productType = "productType";
        public const string c_orders = "orders";
        public const string c_trials = "trials";
        public const string c_recommendations = "recommendations";
        public const string c_emailAddress = "emailAddress";
        public const string c_Date = "Date";
        public const string c_transcriptionStatus = "transcriptionStatus";
        public const string c_transcriptionState = "transcriptionState";
        public const string c_active = "active";
        public const string c_enabled = "enabled";
        public const string c_freeCount = "freeCount";
        public const string c_inactiveReason = "inactiveReason";
        public const string c_planMaxCount = "planMaxCount";
        public const string c_planUsedCount = "planUsedCount";
        public const string c_renewalDate = "renewalDate";
        public const string c_transcriptionSettings = "transcriptionSettings";
        public const string c_smsCount = "smsCount";
        public const string c_speakClearly = "speakClearly";
        public const string c_silentMode = "silentMode";
        public const string c_transcriptionNone = "NONE";
        public const string c_transcriptionInProgress = "IN_PROGRESS";
        public const string c_transcribed = "TRANSCRIBED";
        public const string c_transcriptionError = "ERROR";
        public const string c_transcriptionExceeded = "EXCEED_ALLOTMENT";
        public const string c_transcriptionNotCorrectContact = "NOT_CORRECT_CONTACT";
        public const string c_transcriptionInactivePlan = "INACTIVE_PLAN";
        public const string c_entryIds = "entryIds";
        public const string c_source = "source";
        public const string c_destination = "destination";
        public const string c_length = "length";
        public const string c_callerName = "callerName";
        public const string c_created = "created";
        public const string c_updated = "updated";
        public const string c_messageData = "messageData";
        public const string c_messageDataUrl = "messageDataUrl";
        public const string c_folderId = "folderId";
        public const string c_imageUrl = "imageUrl";
        public const string c_imageSize = "imageSize";
        public const string c_shareKey = "shareKey";
        public const string c_transcript = "transcript";
        public const string c_preview = "preview";
        public const string c_flagged = "flagged";
        public const string c_createSource = "createSource";
        public const string c_imageId = "imageId";
        public const string c_communityGreetingId = "communityGreetingId";
        public const string c_greetingType = "greetingType";
        public const string c_pushRegistration = "pushRegistration";
        public const string c_pushRegistrations = "pushRegistrations";
        public const string c_deviceId = "deviceId";
        public const string c_clientType = "clientType";
        public const string c_pushType = "pushType";
        public const string c_validUntil = "validUntil";
        public const string c_version = "version";
        public const string c_settings = "settings";
        public const string c_userId = "userId";
        public const string c_statuses = "statuses";
        public const string c_playId = "playId";
        public const string c_playType = "playType";
        public const string c_statusType = "statusType";
        public const string c_system = "system";


        public const string c_createdsince = "createdsince";
        public const string c_updatedsince = "updatedsince";

        public const string c_messageboxQuery = "messageboxQuery";
        public const string c_includeExtraInfo = "includeExtraInfo";
        public const string c_offset = "offset";
        public const string c_maxResult = "maxResult";
        public const string c_includeFullCallerName = "includeFullCallerName";
        public const string c_dataFormat = "dataFormat";
        public const string c_forceTranscriptionRequest = "forceTranscriptionRequest";
        public const string c_includeImageUrl = "includeImageUrl";
        public const string c_includeTranscription = "includeTranscription";
        public const string c_updatedFrom = "updatedFrom";
        public const string c_createdFrom = "createdFrom";
        public const string c_updatedUntil = "updatedUntil";
        public const string c_sortBy = "sortBy";
        public const string c_startDate = "startDate";
        public const string c_endDate = "endDate";
        public const string c_deleteType = "deleteType";
        public const string c_groupType = "groupType";
        public const string c_pageNumber = "pageNumber";
        public const string c_pageLength = "pageLength";
        public const string c_contactType = "contactType";
        public const string c_targetUserId = "targetUserId";
        public const string c_inviteType = "inviteType";
        public const string c_group = "group";
        public const string c_playGroup = "playGroup";

        public const string c_account = "account";
        public const string c_phone = "primaryPhoneNumber";
        public const string c_title = "title";
        public const string c_firstName = "firstName";
        public const string c_middleName = "middleName";
        public const string c_lastName = "lastName";
        public const string c_displayName = "displayName";
        public const string c_email = "emailAddress";
        public const string c_password = "password";
        public const string c_pin = "pin";
        public const string c_phoneModel = "phoneModel";
        public const string c_phoneModelWP7 = "51224";
        public const string c_username = "username";
        public const string c_secretWord = "secretWord";
        public const string c_accountTemplate = "accountTemplate";

        public const string c_histories = "histories";
        public const string c_history = "history";
        public const string c_result = "result";
        public const string c_organization = "organization";
        public const string c_avatarId = "avatarId";
        public const string c_avatarData = "avatarData";
        public const string c_homeNumber = "homeNumber";
        public const string c_workNumber = "workNumber";
        public const string c_mobileNumber = "mobileNumber";
        public const string c_pagerNumber = "pagerNumber";
        public const string c_otherNumber1 = "otherNumber1";
        public const string c_otherNumber2 = "otherNumber2";
        public const string c_otherNumber3 = "otherNumber3";
        public const string c_otherNumber4 = "otherNumber4";
        public const string c_blocked = "blocked";
        public const string c_deleted = "deleted";
        public const string c_zipCode = "zipCode";
        public const string c_street = "street";
        public const string c_city = "city";
        public const string c_state = "state";
        public const string c_country = "country";
        public const string c_contact = "contact";
        public const string c_defaultImage = "defaultImage";
        public const string c_baseImage = "baseImage";
        public const string c_contacts = "contacts";
        public const string c_greetingId = "greetingId";
        public const string c_actionType = "actionType";
        public const string c_phonebookSourceType = "phonebookSourceType";
        public const string c_phonebookSourceId = "phonebookSourceId";
        public const string c_alertSettings = "alertSettings";
        public const string c_countryState = "countryState";
        public const string c_testCallDetail = "testCallDetail";
        public const string c_userPhoneNumber = "userPhoneNumber";
        public const string c_outboundPhoneNumber = "outboundPhoneNumber";
        public const string c_initiatedTime = "initiatedTime";
        public const string c_completedTime = "completedTime";
        public const string c_answeredTime = "answeredTime";
        public const string c_IN_SUCCESS = "IN_SUCCESS";
        public const string c_gender = "gender";
        public const string c_aolHandle = "aolHandle";
        public const string c_googleHandle = "googleHandle";
        public const string c_yahooHandle = "yahooHandle";
        public const string c_msnHandle = "msnHandle";
        public const string c_timeZone = "timeZone";
        public const string c_website = "website";
        public const string c_YouMailUser = "YouMail-User";
        public const string c_YouMailPassword = "YouMail-Password";
        public const string c_phoneRecords = "phoneRecords";
        public const string c_phoneRecord = "phoneRecord";
        public const string c_resultStatus = "resultStatus";
        public const string c_displayImageSource = "displayImageSource";
        public const string c_displayImageMedium = "displayImageMedium";
        public const string c_displayImageLarge = "displayImageLarge";
        public const string c_displayImageExtraLarge = "displayImageExtraLarge";

        public const string c_carrier = "carrier";
        public const string c_carriers = "carriers";
        public const string c_carrierClass = "carrierClass";
        public const string c_activeFlag = "activeFlag";
        public const string c_supportedFlag = "supportedFlag";
        public const string c_spamOptions = "spamOptions";
        public const string c_deletedCount = "deletedCount";
        public const string c_errorCount = "errorCount";
        public const string c_exactMatchCount = "exactMatchCount";
        public const string c_ignoredCount = "ignoredCount";
        public const string c_mergedCount = "mergedCount";
        public const string c_newCount = "newCount";
        public const string c_processed = "processed";
        public const string c_ymTotal = "ymTotal";
        public const string c_importingTotal = "importingTotal";
        public const string c_clientRefId = "clientRefId";
        public const string c_expired = "expired";
        public const string c_contactSyncSummary = "contactSyncSummary";
        public const string c_appVersion = "appVersion";
        public const string c_statusCode = "statusCode";
        public const string c_shortMessage = "shortMessage";
        public const string c_longMessage = "longMessage";
        public const string c_timestamp = "timestamp";
        public const string c_timestampMs = "timestampMs";
        public const string c_response = "response";
        public const string c_custom = "custom";
        public const string c_customs = "customs";
        public const string c_key = "key";
        public const string c_registrationStatus = "registrationStatus";
        public const string c_open = "Open";
        public const string c_accountExists = "AccountExists";
        public const string c_validationError = "ValidationError";
        public const string c_virtualNumber = "virtualNumber";
        public const string c_virtualNumbers = "virtualNumbers";
        public const string c_nickname = "nickname";
        public const string c_nickName = "nickName";
        public const string c_additionalPhoneNumbers = "additionalPhoneNumbers";
        public const string c_phoneNumber = "phoneNumber";
        public const string c_autoLoginFlag = "autoLoginFlag";
        public const string c_phoneModelId = "phoneModelId";
        public const string c_primaryPhoneNickname = "primaryPhoneNickname";
        public const string c_primaryPhoneId = "primaryPhoneId";
        public const string c_createTime = "createTime";
        public const string c_includeList = "includeList";
        public const string c_trash = "trash";

        public const string c_greetingImageUrl = "http://www.ymstat.com/dyn/community/img/{0}.png";

        #region API URLs
        public const string c_loginUrl = c_server + "/api/v4/authenticate";
        public const string c_messageboxFoldersUrl = c_server + "/api/v4/messagebox/folders";
        public const string c_messageboxMoveToFolderUrl = c_server + "/api/v4/messagebox/entry/changefolder/{0}/{1}";
        public const string c_messageboxMarkUrl = c_server + "/api/v4/messagebox/entry/status/{0}/{1}/{2}";
        public const string c_messageboxFlagUrl = c_server + "/api/v4/messagebox/entry/flag/{0}/{1}";
        public const string c_historyClearMessage = c_server + "/api/v4/messagebox/entry/history/clear/{0}";
        public const string c_messageBoxEntryQuery = c_server + "/api/v4/messagebox/entry/query";
        public const string c_messageBoxEntryDetails = c_server + "/api/v4/messagebox/entry/{0}/details";
        public const string c_messageBoxHistoryQuery = c_server + "/api/v4/messagebox/entry/history/query";
        public const string c_messageTranscriptionRequest = c_server + "/api/v4/messagebox/entry/{0}/transcript/request";
        public const string c_alertSettingsUrl = c_server + "/api/v4/settings/alertSettings";
        public const string c_setAlertSettingUrl = c_server + "/api/v4/settings/alertSettings/{0}/{1}";
        public const string c_getForwardingInstructions = c_server + "/api/v4/accounts/forwardinginstructions/phonenumber/{0}"; // {phonenumber}
        public const string c_accesspoint = c_server + "/api/v4/accounts/accesspoint/phone/{0}"; // {phonenumber}
        public const string c_getCarrierInfo = c_server + "/api/v4/accounts/carrier/{0}"; // {phonenumber}
        public const string c_setCarrierInfo = c_server + "/api/v4/accounts/carrier/{0}/{1}"; // {phonenumber}/{newcarrierid}
        public const string c_getSupportedCarriers = c_server + "/api/v4/lookup/carrier/all/supported";
        public const string c_pollMessages = c_pollServer + "/vm/poll/{0}/{1}"; // {lastdigitUserId}/{userId}

        public const string c_getContacts = c_server + "/api/v4/contacts/query";
        public const string c_contactsSelect = c_server + "/api/v4/contacts/{0}";
        public const string c_setContactGreeting = c_server + "/api/v4/contacts/{0}/greeting/{1}";
        public const string c_setContactAction = c_server + "/api/v4/contacts/{0}/action/{1}";
        public const string c_getOrCreateContact = c_server + "/api/v4/contacts/getorcreate/{0}";
        public const string c_uploadContacts = c_server + "/api/v4/contacts/upload";
        public const string c_uploadContactsStatus = c_server + "/api/v4/contacts/upload/status";

        public const string c_getGreetings = c_server + "/api/v4/greetings/query";
        public const string c_deleteGreeting = c_server + "/api/v4/greetings/{0}";
        public const string c_statusGreetings = c_server + "/api/v4/greetings/status";
        public const string c_registrationVerify = c_server + "/api/v4/accounts/registration/verify/{0}";
        public const string c_createAccountUrl = c_server + "/api/v4/accounts";
        public const string c_accountDetailsUrl = c_server + "/api/v4/accounts/phone/{0}";
        public const string c_settingsUserId = c_server + "/api/v4/settings/";

        public const string c_ecommerce = c_server + "/api/v4/status/accountStatus/ecommerceStatus";
        public const string c_trialStatus = c_server + "/api/v4/status/accountStatus/ecommerceStatus/trials";
        public const string c_getPlans = c_server + "/api/v3/store/products";
        public const string c_transcriptionStatusApi = c_server + "/api/v4/status/transcriptionStatus";
        public const string c_transcriptionSettingsApi = c_server + "/api/v4/settings/transcriptionSettings";
        public const string c_transcriptionSettingsSet = c_server + "/api/v4/settings/transcriptionSettings/{0}/{1}";
        public const string c_addProduct = c_server + "/api/v3/store/products/purchase/{0}";
        public const string c_createVoicemail = c_server + "/api/v4/messagebox/entry/create";
        public const string c_createFolder = c_server + "/api/v4/messagebox/folders";
        public const string c_getFolder = c_server + "/api/v4/messagebox/folders";
        public const string c_moveAllMessagesFromFolder = c_server + "/api/v4/messagebox/{0}/moveentries/{1}";
        public const string c_deleteFolder = c_server + "/api/v4/messagebox/folders/{0}";

        public const string c_createContact = c_server + "/api/v4/contacts";
        public const string c_updateContact = c_server + "/api/v4/contacts/{0}";

        public const string c_devicePushRegistration = c_server + "/api/v4/settings/alertSettings/pushRegistrations/{0}";
        public const string c_devicePushRegistrations = c_server + "/api/v4/settings/alertSettings/pushRegistrations";
        public const string c_deviceUpdateRegistration = c_server + "/api/v4/settings/alertSettings/pushRegistrations/{0}/renew/{1}";

        public const string c_verifyCallSetupInitiate = c_server + "/api/v4/tui/call/?phoneNumber={0}";
        public const string c_verifyCallSetupValidate = c_server + "/api/v4/tui/call/result/mostrecent";

        public const string c_confirmTextMessages = c_server + "/api/v4/accounts/confirmation/{0}";
        public const string c_confirmTextMessagesValidation = c_server + "/api/v4/accounts/confirmation/{0}/{1}";

        public const string c_directoryLookup = "www.youmail.com/phone-lookup/number/{0}?xml";

        public const string c_spamSettings = c_server + "/api/v4/messagebox/settings/spam";
        public const string c_spamSettingsSet = c_server + "/api/v4/messagebox/settings/spam/{0}";

        public const string c_extraLines = c_server + "/api/v4/virtualnumbers";
        public const string c_getVirtualNumber = c_server + "/api/v4/virtualnumbers/phone/{0}"; // {phonenumber}

        #endregion API URLs

        #region WebPage APIs

        public const string c_getGreetingsUrl = "https://greetings.youmail.com/?auth={0}";
        public const string c_greetingUrl = "www.youmail.com/mcs/greeting/greeting.do?id={0}&authtoken={1}";
        public const string c_storeUrl = "https://store.youmail.com/store/home.do?auth={0}";
        public const string c_storeProductUrl = "https://store.youmail.com/store/cart/productAddCart.do?sku={0}&auth={1}";
        public const string c_recoverUrl = "https://www.youmail.com/login/user/password/recover";
        public const string c_receiptsUrl = "https://www.youmail.com/youmail/user/autoreply?auth={0}";
        public const string c_spamUrl = "https://www.youmail.com/youmail/user/spam/edit?auth={0}";
        public const string c_TOSUrl = "http://www.youmail.com/termsofuse.html";
        public const string c_PrivacyUrl = "http://www.youmail.com/home/corp/privacy";
        public const string c_greetingStatus = "https://www.youmail.com/youmail/greeting/status/edit?auth={0}";
        public const string c_changePin = "https://www.youmail.com/youmail/user/password/edit?auth={0}";
        public const string c_userProfile = "https://www.youmail.com/youmail/user/personal?auth={0}";
        public const string c_userPhones = "https://www.youmail.com/youmail/user/phones?auth={0}";
        public const string c_webLinkVirtualNumbers = "https://www.youmail.com/youmail/user/virtual-numbers?auth={0}";
        public const string c_paymentMethods = "https://store.youmail.com/store/cart/payment?auth={0}&r=/youmail/user/statements";
        #endregion

        public const string c_ditchedCall = "ditchedCall";
        public const string c_ditchedCallMask = "ditchedCallMask";
        public const string c_emailAttachment = "emailAttachment";
        public const string c_emailAttachmentType = "emailAttachmentType";
        public const string c_emailCustomFormat = "emailCustomFormat";
        public const string c_emailFormat = "emailFormat";
        public const string c_missedCall = "missedCall";
        public const string c_missedCallMask = "missedCallMask";
        public const string c_newMessage = "newMessage";
        public const string c_newMessageMask = "newMessageMask";
        public const string c_newSpamMessage = "newSpamMessage";
        public const string c_newSpamMessageMask = "newSpamMessageMask";
        public const string c_pushConditions = "pushConditions";
        public const string c_emailCCEnabled = "emailCCEnabled";
        public const string c_emailCCList = "emailCCList";
        public const string c_confirmedSmsPhone = "confirmedSmsPhone";
        public const string c_carrierSupportsSms = "carrierSupportsSms";

        public const string c_availableFlag = "availableFlag";
        public const string c_shortDescription = "shortDescription";
        public const string c_fulfilledFlag = "fulfilledFlag";
        public const string c_sku = "sku";

        public const string c_errorInvalidResponse = "An invalid response was returned.";

        public const string c_YouMailAPICall = "YouMailAPICall";
        public const string c_YouMailAPICalls = "YouMailAPICalls";
        public const string c_YouMailAPICallURI = "URI";
        public const string c_YouMailAPICallURIData = "URIData";
        public const string c_YouMailAPICallVerb = "Verb";
        public const string c_YouMailAPICallAuth = "Auth";
    }
}
