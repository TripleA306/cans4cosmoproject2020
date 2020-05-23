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

//open browser with a url of "http://localhost:8080"
'Set the phone number'
WebUI.setText(findTestObject('Object Repository/SignUp_OR/Text Inputs/txtPhoneNumber'), '11111111111')

WebUI.verifyElementPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorPhoneNumber'), 0)

'Go to the next page'
WebUI.click(findTestObject('SignUp_OR/Buttons/btnGenInfoNext'))

WebUI.verifyElementPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorPhoneNumber'), 0)

'Ensure the error message exists'
WebUI.verifyElementText(findTestObject('SignUp_OR/Error Boxes/errorPhoneNumber'), 'Phone number must be 10 digits long')



