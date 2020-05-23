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

//This test will esnure that there is an error message shown to inform the Subscriber
//That there are no collections completed for their account.
//open a new browser to log into another account
//close the browser
WebUI.closeBrowser()
WebUI.openBrowser('http://localhost:8080/#/')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))

//Switch to the google authentication popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'CosmoNoRegion@gmail.com')

//click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))

//Sleep so it doesnt go to fast and fail
Thread.sleep(2500)

//enter the Cans4Cosmo@gmail.com password into the password field
WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'mcTdnBtnMyqgoBB2xSEK7w==') //Cosmo2020

//focus back onto the google popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

Thread.sleep(2000)

//wait for the next button to appear on the google authentication popup
WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)

//Click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'))

//Switch to the Cans4Cosmo application
WebUI.switchToWindowTitle('Cans4Cosmo')

//verify that the Welcome header is present on the page
WebUI.verifyElementPresent(findTestObject('HomePage_OR/lblWelcome'), 5)

//Open the hamburger menu
WebUI.click(findTestObject('Object Repository/Welcome_OR/btnHamburgerMenu'))

//click the collection menu item
WebUI.click(findTestObject('Object Repository/Welcome_OR/btnCollectionHistory'))

//verify that the URL matches the expected url for the collections page.
WebUI.verifyMatch(WebUI.getUrl(), 'http://localhost:8080/#/home/history', false)

//verify that the error message exists on the page
WebUI.verifyTextPresent("There are no records to show", false, FailureHandling.STOP_ON_FAILURE)


