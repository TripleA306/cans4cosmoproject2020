import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import java.sql.Time as Time
import java.time.LocalDate as LocalDate
import java.time.format.DateTimeFormatter as DateTimeFormatter
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

//This test will create a new region with all correct entries and ensure the object is created
WebUI.click(findTestObject('Regions_OR/Navigation_Bar/btnRegions'))

WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_AddRegion'))

WebUI.verifyTextNotPresent('Region name is required', false)

WebUI.verifyTextNotPresent('Collection frequency is required', false)

WebUI.verifyTextNotPresent('First pickup date is required', false)

WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), 'Test')

WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), '7')

WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_FirstCollection'), LocalDate.now().format(DateTimeFormatter.ISO_LOCAL_DATE))

WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_SaveChanges'))

//Go to Page 2
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/btn_pagination_Page2'))

WebUI.click(findTestObject('Object Repository/Regions_OR/Region_List/btnPage3'))

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liTestName'), 2)

WebUI.verifyElementText(findTestObject('Regions_OR/Region_List/liTestName'), 'Test')
