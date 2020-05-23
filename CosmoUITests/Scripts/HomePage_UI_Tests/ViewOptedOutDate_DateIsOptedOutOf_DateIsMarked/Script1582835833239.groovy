import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import internal.GlobalVariable as GlobalVariable

/**
 * This Test will send a request to the API to opt out of a subscriber's upcoming pickup
 */

//Admin Page

//Open the browser
WebUI.openBrowser('http://localhost:8081')

//verify subscriber option on the nav bar exists
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'), 5)

//click the subscriber option on the nav bar
WebUI.click(findTestObject('Page_Admin - Subscribers/Navigation_Bar/button_Subscriber_Nav'))

//Verify that the subscriber table is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/tbl_Subscriber_Table'), 5)

//Verify that the subscriber Header is present
WebUI.verifyElementPresent(findTestObject('Page_Admin - Subscribers/Subscriber_Table/h1_Subcriber_Header'), 5)

//click the button which will force an opt out of a subscriber's upcoming pickup
WebUI.click(findTestObject('Page_Admin - Subscribers/Subscriber_Table/btn_OptOut_Sub1')) // ****************** Test Object not created yet

//Subscriber Page

//navigate to url of "http://localhost:8080"
WebUI.navigateToUrl('http://localhost:8080')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))

//Switch to the google authentication popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//Enter "Cans4Cosmo@gmail.com" into the email field
WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'NathanIsSuperCool101@gmail.com')

//click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))

//Sleep so it doesnt go too fast and fail
Thread.sleep(2500)

//enter the Cans4Cosmo@gmail.com password into the password field
WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'qRr9/hiJpFFqn26WtPGbFg==') //NathanIsSoCool
    

//focus back onto the google popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//wait for the next button to appear on the google authentication popup
WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)

//Click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'))

//Switch to the Cans4Cosmo application
WebUI.switchToWindowTitle('Cans4Cosmo')

//Sleep so it doesnt go too fast and fail
Thread.sleep(2500)

//Verify that the opted out label is present on the page
WebUI.verifyElementPresent(findTestObject('Object Repository/HomePage_OR/lblNextCollectionDate1_OptedOut'), 5) // ******************** Test Object needs to be created

//close the browser
WebUI.closeBrowser();
