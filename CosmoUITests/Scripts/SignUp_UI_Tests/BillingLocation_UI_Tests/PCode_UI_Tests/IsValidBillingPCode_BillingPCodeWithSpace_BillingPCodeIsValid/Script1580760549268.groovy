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
'Enter a valid address'
WebUI.setText(findTestObject('SignUp_OR/Text Inputs/txtAddress'), '123 1st Street')

WebUI.verifyElementNotPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorAddress'), 0)

'Enter a valid city'
WebUI.setText(findTestObject('SignUp_OR/Text Inputs/txtCity'), 'Saskatoon')

WebUI.verifyElementNotPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorCity'), 0)

'Set a valid postal code'
WebUI.setText(findTestObject('SignUp_OR/Text Inputs/txtPCode'), 'S7N 4T5')

WebUI.verifyElementNotPresent(findTestObject('SignUp_OR/Error Boxes/errorPostalCode'), 0)

'Check the "different billing location" checkbox'
WebUI.check(findTestObject('SignUp_OR/Check Boxes/chkDifferentBillingLocation'))

WebUI.click(findTestObject('SignUp_OR/Buttons/btnPickUpLocationNext'))

'Ensure no error message exists'
WebUI.verifyElementNotPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorAddress'), 0)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorCity'), 0)

WebUI.verifyElementNotPresent(findTestObject('SignUp_OR/Error Boxes/errorPostalCode'), 0)


'Set the address to something too long'
WebUI.setText(findTestObject('SignUp_OR/Text Inputs/txtBillingAddress'), 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa')

WebUI.verifyElementPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorBillingAddress'), 0)

WebUI.click(findTestObject('Object Repository/SignUp_OR/Buttons/btnBillingLocationSave'))

WebUI.verifyElementPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorBillingAddress'), 0)

'Ensure the error message exists'
WebUI.verifyElementText(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorBillingAddress'), 'Address must be between 2 and 60 characters')

