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

//verify that the google authentication popup is present
WebUI.verifyElementPresent((findTestObject('Login_OR/GoogleAuthPopup/GoogleAuthPopup')),5)

//close the browser
WebUI.closeBrowser()

