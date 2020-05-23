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

//This test will ensure that the listed regions are present and in the correct order
WebUI.click(findTestObject('Regions_OR/Navigation_Bar/btnRegions'))
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'))

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liS90PastRegion'), 2)

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liDowntown'), 2)

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liHarborCreek'), 2)

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liIceborne'),2)

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liS90ActiveRegion'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liMyNameIsBrett'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liNathans2ndTestRegion'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liNathansTestRegion'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liSouthEnd'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liNewWorld'),2)

WebUI.click(findTestObject('Regions_OR/Region_List/btnPage2'))

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liS90InactiveRegion'), 2)

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liMyNameIsBrett'), 2)

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liNathans2ndTestRegion'), 2)

WebUI.verifyElementPresent(findTestObject('Regions_OR/Region_List/liNathansTestRegion'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liSouthEnd'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liNewWorld'),2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liS90PastRegion'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liDowntown'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liHarborCreek'), 2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liIceborne'),2)

WebUI.verifyElementNotPresent(findTestObject('Object Repository/Regions_OR/Region_List/liS90ActiveRegion'), 2)

WebUI.click(findTestObject('Object Repository/Regions_OR/Region_List/btnPage3'))

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liSouthEnd'), 2)

WebUI.verifyElementPresent(findTestObject('Object Repository/Regions_OR/Region_List/liNewWorld'),2)

