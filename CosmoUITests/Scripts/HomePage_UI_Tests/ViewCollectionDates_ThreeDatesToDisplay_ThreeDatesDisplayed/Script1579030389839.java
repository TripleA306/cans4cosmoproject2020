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

//open browser
WebUI.openBrowser('http://localhost:8080/')

//click continue with google button
//WebUI.click(findTestObject('Object Repository/ContinueWithGoogleButton'))
//setting the text of the email text field in the google popup sign in form
//WebUI.setText(findTestObject('Object Repository/GoogleEmailTextField'), "123@gmail.com")
//click next button on the google popup sign in form
//WebUI.click(findTestObject('Object Repository/GoogleNextButton1'))
//setting the text of the password text field in the google popup sign in form
//WebUI.setText(findTestObject('Object Repository/GooglePasswordTextField'), "P@ssw0rd")
//click next button on the google popup sign in form
//WebUI.click(findTestObject('Object Repository/GoogleNextButton2'))
//Verify the home page is properly loading
//Make sure welcome label is correct
WebUI.verifyElementPresent(findTestObject('HomePage_OR/lblWelcome'), 0)

WebUI.verifyElementText(findTestObject('HomePage_OR/lblWelcome'), 'Welcome, Nathan')

//Make sure NextCollectionDates label is correct
WebUI.verifyElementPresent(findTestObject('HomePage_OR/lblNextCollectionDates'), 0)

WebUI.verifyElementText(findTestObject('HomePage_OR/lblNextCollectionDates'), 'Your Next Collection Dates:')

//Verify that the date 1s text field is displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate1'), 0)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate1'), '1. February 24, 2020')

//Verify that the date 2s text field is displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate2'), 0)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate2'), '2. May 04, 2020')

//Verify that the date 3s text field is displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate3'), 0)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate3'), '3. July 13, 2020')

//close the browser
WebUI.closeBrowser()

