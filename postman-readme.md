# Postman token-authentication scenario #

### Summary ###

Covering request samples for authenticating with Postman, in order to be able to access resource endpoints a.k.a '/api/todos'.

### Solution ###
Solution | Author(s)
---------|----------
OpenIddict.Mongo | Ilker Karimanov

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Requests #
**1. Register a client**

Url(POST): 

    http://localhost:5000/api/clients

Headers:

    Content-type: application/json

Body(application/json):

    {
    "key":"idddWeb",
    "name":"idddWeb",
    "applicationType":"1",
    "origin":"http://localhost:5000",
	"redirectUri":"http://localhost:5000",
	"logoutRedirectUri": "http://localhost:5000",
	"confirmationUri": "http://localhost:5000/confirm"
    } 

**2. Register a user**

Url(POST): 

    http://localhost:5000/api/account/register

Headers:

    Content-type: application/json

Body(application/json):

    {
	    "email":"testme@gmail.com",
	    "clientKey":"idddWeb",
    }

**3. Create a password**

Url(POST): 

    http://localhost:5000/api/account/password

Headers:

    Content-type: application/json

Body(application/json):

    {
	    "username":"testme@gmail.com",
	    "password":"Pa$$w0rd"
	
    }

**4. Authenticate/login request**

Url(POST): 

    http://localhost:5000/connect/token

Headers:

    Content-type: application/x-www-form-urlencoded

Body(x-www-form-urlencoded):

    
	    grant_type: password
	    username:testme@gmail.com
	    password: Pa$$w0rd
        client_id: idddWeb
        scope: offline_access

Response tokens:

  "token_type": "Bearer",
  "access_token": "CfDJ8HkADOaYlbBIsaKIfogqBefGjAMzG0TFOO9AgeiMCrgR1n_z613N1O1R8NC-n3Mua74oa_nnYzdmFlAmph1URsnRcM3cjCmw_wR3-YqqI5lLsIDkQEYReqbsOsKr-5qYcj8xUxN40zMRzxczPn8DD-7L2Vf_Q1pK4_Lk9bLB13881jqKmeBPrz2rLWfc5iP06Px0EIaSKPThwMHva4d3jFa7Hy3-ZT92tWB2LcJf967tgjWZdovLH0ex-O2mAfGsMNy9zVUG_d1BL8frCaqSZzG6zOODYULAEgepaXAXEOZh91MjEN03dhKfXrEKtN_9ItE6_QVlA5m1UZqyI2xofnbkqVAOtfDQlzsD-IVIzK6YbgwmLDR7J4bKoQ5T7HZGqShsuHvuxsWKzr9qXj-BlCLAHuwZThofHuvLzkVCEpxESkFUecHvvhQ0z1P-GURqR2SvT5tKCu8sfvKigXn3DKoLmzuLX0kz_rbRjzjhCqJT58W85Eymf857e-PHmYrabZYBrCE5UJ6RqGp-uEFzvcLwWk4fziJqGtQkYPVrge8BT7i7YIyIs-B3GS1oJVqhICi8YNkJ1M6DDhHDrrSnec81D_jGzGlaUNxZ493NZHeAbTpQl5WOKAkIf_9dwt-4is_pG1gQ1yplfpT_2RdnNfQniId0N8v3kHgo2AUQpIl0gx3Zlic3TraE4CTZ2AWwEj8D1AZTjD0wBuKaFAkKhkRZyjBxu89mDo6qk0p-Y80sML9dozpe2DNQiOZubYgejjmq1nSPCHVIT3dr-6dkf1w9GT5DCQoGFksU84tcZrFm",
  "expires_in": 900,
  "refresh_token": "CfDJ8HkADOaYlbBIsaKIfogqBef9GLkbs-Zx9EafopYy9_keWXfmt8PfE1sNrXbbc853l1nsxTEM0hkPwsWize2u6nFPpJgzKl-EYZQnWtmn2auciwunJ_JucxV_J_r4grtkU3e_v1BQtSi_TTYc5gw7P_Sl5UHwc4s6wKd_hw_vQtdw2iUTLDRvOUwdjze5-iVVLATx7vc44yZ1pTWlIHb4uAk-JGL7sPZ8fbFU9g7OD-SnEY4aYDzfaPG7aUX-CImc6i_biA5z-pX0ozxIUfT_l9HxEOWsrSJ0CkIQiqhqEpsmm5uzCaeF_AlLgC1XiOBA7AY19nna6t5LSET0y6aMAKntjTHIaaJwI_T7eOeqoxI8NRrMz79Mr1qktpprgDEvR_b-s1Vq1xWOOD70hBYN2r6elvp00-IGZ-BbdXvtJg5DweGVzxZL4MteXoDY2q6loFR8kBfOzxPuTamIn__-ylx4w1Lnu-nuY794R57IQAzseWNRNEV3R1OFyd00S0Si_DpEghg038yQAv8UzV0aMliCit9JD2nPj4VJZ4N5o5wFpuA4KMGGEj3RKpOyrgZkOXYhik6yt3PK7QSVdJKIR7C0Pe4VA1l_J80uWHmABryvQ2BCSzEUiQC_PwTzwtzZwVzj5mNa4j6m39qBpD1m5REZmLou-mT5ddRNyHJ_HMyY2-xfYgJoByQfdQuvngFUx0jUVkjjz-cKHJWS280QWbSOvGFMvNQQctqZhFW9kVzkPsfJ6uWlfQ0v4hmPnipcjQ",
  "as:clientAllowedOrigin": "http://localhost:5000"   

**Refresh token request**: 

Url(POST): 

    http://localhost:5000/connect/token

Headers:

    Content-type: application/x-www-form-urlencoded

Body(x-www-form-urlencoded):

    
	    grant_type: refresh_token_
	    client_id: idddWeb
        refresh_token: [your refresh token]
**Call authorized endpoints**

Url(GET): 

    http://localhost:5000/api/todos

Headers:

    Content-type: application/json
    Authorization: Bearer [your access token]







