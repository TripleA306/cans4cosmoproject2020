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
import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

//This test will check the for an entry below the lower bound of allowable collection frequency

WebUI.click(findTestObject('Regions_OR/Navigation_Bar/btnRegions'))

WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_AddRegion'))

WebUI.verifyTextNotPresent('Region name is required', false)

WebUI.verifyTextNotPresent('Collection frequency is required', false)

WebUI.verifyTextNotPresent('First pickup date is required', false)

WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), '0')

WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_SaveChanges'))

WebUI.verifyTextPresent('Collection frequency must be between 1 and 52', false)

WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_DiscardChanges'))