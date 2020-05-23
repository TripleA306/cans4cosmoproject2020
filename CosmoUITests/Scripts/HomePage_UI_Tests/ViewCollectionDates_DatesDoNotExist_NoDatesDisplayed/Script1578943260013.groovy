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
import java.util.Date as Date
import java.text.SimpleDateFormat as SimpleDateFormat

///////////////////////////////////////////////////////////////////////////////
//	This test class will be to simulate the case in which the Subscribers 
//	Location is not yet added to a region 
//	This will result in no dates displaying  
///////////////////////////////////////////////////////////////////////////////
//open browser with a url of "http://localhost:8080"
WebUI.openBrowser('http://localhost:8080')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))

//Switch to the google authentication popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//Enter "Cans4Cosmo@gmail.com" into the email field
WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'CosmoNoRegion@gmail.com')

//click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))

//Sleep so it doesnt go to fast and fail
Thread.sleep(2500)

//enter the Cans4Cosmo@gmail.com password into the password field
WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'mcTdnBtnMyqgoBB2xSEK7w==') //Cosmo2020
    

//focus back onto the google popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//wait for the next button to appear on the google authentication popup
WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)

//Click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'))

//Switch to the Cans4Cosmo application
WebUI.switchToWindowTitle('Cans4Cosmo')

//Sleep so it doesnt go to fast and fail
Thread.sleep(2500)

//Verify the first name of the subscriber is displayed
WebUI.verifyElementText(findTestObject('HomePage_OR/lblWelcome'), 'Welcome, NathanNoRegion')

WebUI.click(findTestObject('Object Repository/Welcome_OR/btnHamburgerMenu'))

WebUI.click(findTestObject('Object Repository/Welcome_OR/btnDashboard'))


//Make sure NextCollectionDates label is present
WebUI.verifyElementPresent(findTestObject('Object Repository/HomePage_OR/lblNextCollectionDates'), 2)

//Make sure the text is displayed as designed 
WebUI.verifyElementText(findTestObject('Object Repository/HomePage_OR/lblNextCollectionDates'), 'Your Next Collection Dates:')

//Verify that the date 1s text field is there and displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate1'), 2)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate1'), '1.')

//Verify that the date 2s text field is there and displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate2'), 2)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate2'), '2.')

//Verify that the date 3s text field is there and displayed correctly
WebUI.verifyElementPresent(findTestObject('HomePage_OR/txtNextCollectionDate3'), 2)

WebUI.verifyElementText(findTestObject('HomePage_OR/txtNextCollectionDate3'), '3.')

//close the browser
WebUI.closeBrowser()

