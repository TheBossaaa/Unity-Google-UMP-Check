using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class UMP : MonoBehaviour
{
    [SerializeField] Button privacyButton;
    [SerializeField] Button resetButton;

    public AdmobManager admobManager;

    void Start()
    {

        ConsentStart();

        GDPRDebugger();
    }

    void ConsentStart()
    {

        Debug.Log("Consent Start initated ");

        // Set tag for under age of consent.
        // Here false means users are not under age of consent.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,

        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void Update()
    {

        // Enable the privacy settings button.
        if (privacyButton != null)
        {
            privacyButton.onClick.AddListener(UpdatePrivacyButton);
            // Disable the privacy settings button by default.
            // privacyButton.interactable = false;
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetConsentState);
        }

    }

    void ResetConsentState()
    {
        ConsentInformation.Reset();
        Debug.Log("Consent Reset Initiated ");
    }

    private void UpdatePrivacyButton()
    {
        ShowPrivacyOptionsForm();
    }

    void OnConsentInfoUpdated(FormError consentError)
    {
        Debug.Log("Consent Info Updated initiated ");

        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                Debug.Log("Consent Form Gathering Failed ");
                admobManager.RequestAds();
                return;
            }

            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    // TODO: Request an ad.
                    admobManager.RequestAds();
                });
            }
        });
    }

    /// <summary>
    /// Shows the privacy options form to the user.
    /// </summary>
    public void ShowPrivacyOptionsForm()
    {
        Debug.Log("Showing privacy options form.");

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
        {
            if (showError != null)
            {
                Debug.LogError("Error showing privacy options form with error: " + showError.Message);
            }
            // Enable the privacy settings button.
            if (privacyButton != null)
            {
                privacyButton.interactable =
                    ConsentInformation.PrivacyOptionsRequirementStatus ==
                    PrivacyOptionsRequirementStatus.Required;
            }
        });
    }


    void GDPRDebugger()
    {
        ///Summary
        ///Use this for debugging
        ///

        // Define the test device ID for debugging
        string testDeviceHashedId = "0B030C0B27FA3A0A7FCF5766D3BBBA1A"; // Replace with your actual test device ID

        // Create debug settings for consent testing
        var debugSettings = new ConsentDebugSettings
        {
            TestDeviceHashedIds = new List<string>
        {
            testDeviceHashedId
        }
        };

        // Set the debug geography for testing in the EEA
        debugSettings.DebugGeography = DebugGeography.EEA;

        // Set tag for under the age of consent.
        // Here false means users are not under the age of consent.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
            ConsentDebugSettings = debugSettings,
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

}
