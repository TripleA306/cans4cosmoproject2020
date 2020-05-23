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
import org.openqa.selenium.Keys as Keys

//open browser with a url of "http://localhost:8080"
WebUI.openBrowser('http://localhost:8080')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnContinueWithGoogle'))

//Switch to the google authentication popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//Enter "Cans4Cosmo@gmail.com" into the email field
WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'Cans4Cosmo@gmail.com')

//click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))

//enter an incorrect password for Cans4Cosmo@gmail.com into the password field
WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'KJF0cIDQX0qhkx6D3aR8Dg==')

//wait for the next button to appear on the google authentication popup
WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)

Thread.sleep(2000)

//Click on the next button
WebUI.click(findTestObject('Object Repository/Login_OR/GoogleAuthPopup/btnNext2'))

//verify that the error message appears for the password field
WebUI.verifyElementPresent(findTestObject('Login_OR/GoogleAuthPopup/lblErrorWrongPassword'), 5)

//close the browser
WebUI.closeBrowser()

