import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.checkpoint.CheckpointFactory as CheckpointFactory
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testcase.TestCaseFactory as TestCaseFactory
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testdata.TestDataFactory as TestDataFactory
import com.kms.katalon.core.testobject.ObjectRepository as ObjectRepository
import com.kms.katalon.core.testobject.TestObject as TestObject

import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile

import internal.GlobalVariable as GlobalVariable

import com.kms.katalon.core.annotation.SetUp
import com.kms.katalon.core.annotation.SetupTestCase
import com.kms.katalon.core.annotation.TearDown
import com.kms.katalon.core.annotation.TearDownTestCase

/**
 * Some methods below are samples for using SetUp/TearDown in a test suite.
 */

/**
 * Setup test suite environment.
 */
@SetUp(skipped = false) // Please change skipped to be false to activate this method.
def setUp() {
	// Put your code here.
	WebUI.openBrowser("http://localhost:5002/api/WebApi/reloadDB")
	
	WebUI.navigateToUrl("http://localhost:8080")
	
	//Click on the Continue with Google Button
	WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))
	
	//Switch to the google authentication popup
	WebUI.switchToWindowTitle('Sign in – Google accounts')
	
	//Enter "Cans4CosmoTest@gmail.com" into the email field
	WebUI.setText(findTestObject('Login_OR/GoogleAuthPopup/inputEmail'), 'Cans4CosmoTest2@gmail.com')
	
	//click on the next button
	WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext1'))
	
	Thread.sleep(2500)
	
	//enter the Cans4CosmoTest2@gmail.com password into the password field
	WebUI.setEncryptedText(findTestObject('Login_OR/GoogleAuthPopup/inputPassword'), 'mcTdnBtnMyqgoBB2xSEK7w==')
	
	//focus back onto the google popup
	WebUI.switchToWindowTitle('Sign in – Google accounts')
	
	//wait for the next button to appear on the google authentication popup
	WebUI.waitForElementPresent(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'), 5)
	
	//focus back onto the google popup
	WebUI.switchToWindowTitle('Sign in – Google accounts')
	
	//Click on the next button
	WebUI.click(findTestObject('Login_OR/GoogleAuthPopup/btnNext2'))
	
	//Switch to the Cans4Cosmo application
	WebUI.switchToWindowTitle('Cans4Cosmo')
}

/**
 * Clean test suites environment.
 */
@TearDown(skipped = true) // Please change skipped to be false to activate this method.
def tearDown() {
	// Put your code here.
	WebUI.closeBrowser('http://localhost:8080')
}

/**
 * Run before each test case starts.
 */
@SetupTestCase(skipped = false) // Please change skipped to be false to activate this method.
def setupTestCase() {

WebUI.navigateToUrl('http://localhost:8080/#/')

//Click on the Continue with Google Button
WebUI.click(findTestObject('Login_OR/LoginPage/GoogleAuthPopup'))


//Switch to the Cans4Cosmo application
WebUI.switchToWindowTitle('Cans4Cosmo')
}

/**
 * Run after each test case ends.
 */
@TearDownTestCase(skipped = true) // Please change skipped to be false to activate this method.
def tearDownTestCase() {
	// Put your code here.
}

/**
 * References:
 * Groovy tutorial page: http://docs.groovy-lang.org/next/html/documentation/
 */