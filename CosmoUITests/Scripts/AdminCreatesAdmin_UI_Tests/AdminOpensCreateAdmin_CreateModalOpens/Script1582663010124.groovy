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
 * This tests the functionality of the Displaying the Create Admin form on the Admin Web Page
 * Authors: Nathan Kappel cst217, Aaron Atkinson cst201
 */

//Open the browser
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/btnCreateAdmin'), 5)

//Click on the Create Admin Button
WebUI.click(findTestObject('Object Repository/AdminCreateAdmin_OR/btnCreateAdmin'))

//Verify the Create Admin Modal is displayed the to user
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/lblCreateAdmin'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/lblUsername'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/lblPassword'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/lblConfirmPassword'), 5)

//Verify that the entry fields are present
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/eUsername'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/ePassword'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/eConfirmPassword'), 5)

//Verify that the buttons are present
WebUI.verifyElementPresent(findTestObject('AdminCreateAdmin_OR/btnClose'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/AdminCreateAdmin_OR/btnSave'), 5)

//Click the cancel button
WebUI.click(findTestObject('AdminCreateAdmin_OR/btnClose'));

Thread.sleep(1000);