import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable

WebUI.openBrowser('http://localhost:8080')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))

//Switch to the google authentication popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//Enter "Cans4Cosmo@gmail.com" into the email field
WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'Cans4Cosmo@gmail.com')

//click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))

//Sleep so it doesnt go to fast and fail
Thread.sleep(2500)

//enter the Cans4Cosmo@gmail.com password into the password field
WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'mcTdnBtnMyqgoBB2xSEK7w==' //Cosmo2020
    //Cosmo2020
    //Cosmo2020
    //Cosmo2020
    )

//focus back onto the google popup
WebUI.switchToWindowTitle('Sign in – Google accounts')

//wait for the next button to appear on the google authentication popup
WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)

//Click on the next button
WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'))

//Switch to the Cans4Cosmo application
WebUI.switchToWindowTitle('Cans4Cosmo')

//Open the Hamburger menu to see the logout button
WebUI.click(findTestObject('Object Repository/Welcome_OR/btnHamburgerMenu'))

WebUI.click(findTestObject('Object Repository/Logout_OR/btnLogOut'))

//If the Google button is present, we have signed out
WebUI.verifyTextPresent('Login', false)

