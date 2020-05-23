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
WebUI.setText(findTestObject('SignUp_OR/Text Inputs/txtBillingCity'), 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa')

WebUI.click(findTestObject('Object Repository/SignUp_OR/Buttons/btnBillingLocationSave'))

WebUI.verifyElementPresent(findTestObject('Object Repository/SignUp_OR/Error Boxes/errorBillingCity'), 0)

'Ensure the error message exists'
WebUI.verifyElementText(findTestObject('SignUp_OR/Error Boxes/errorBillingCity'), 'City must be between 2 and 60 characters')

