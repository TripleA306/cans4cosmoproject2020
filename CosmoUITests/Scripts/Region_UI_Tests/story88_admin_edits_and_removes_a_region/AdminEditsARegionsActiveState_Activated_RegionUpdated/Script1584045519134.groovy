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

String expectedInactiveRegion = "Kyle's S90 Inactive Region"

//Click on the Region Nav bar to navigate to the Region Page
WebUI.click(findTestObject('Object Repository/Admin_Regions/btn_Region_Nav'))

//Verify that the region table, and the first 5 regions are present
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/RegionTable/tbl_RegionTable'), 5)

WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_item1'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item2'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item3'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item4'), 5)
WebUI.verifyElementPresent(findTestObject('Admin_Regions/RegionTable/td_Item5'), 5)

//Verify that the region details pane exists
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/DetailsPaneDiv'), 5)

//Verify that the Region show all Button is visible
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'), 5)

//Click on the first Region on the table
WebUI.click(findTestObject('Admin_Regions/RegionTable/td_item1'))

//Verify that the details pane displays the fields
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_RegionName'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_Frequency'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_FirstCollection'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_FirstCollection'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionLabel'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionDate'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_Active'), 5)
WebUI.verifyElementPresent(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/swt_Active'), 5)

//Click on the showall button to show all Regions
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'))

//Verify that the inactive region is present and is unchecked
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item3'))
String inactiveRegion =  WebUI.getAttribute(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), "value")
WebUI.verifyMatch(inactiveRegion, expectedInactiveRegion , false)
WebUI.verifyElementNotChecked(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/swt_Active'), 5)

//Set the Region back to Active
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_EditRegion'))
WebUI.click(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/swt_Active'))
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_SaveChanges'))

//Wait a bit
Thread.sleep(1000)

//Verify that the Region is active by viewing all active regions
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_ShowAll'))
WebUI.click(findTestObject('Object Repository/Admin_Regions/RegionTable/td_Item2'))
String nowActiveRegion =  WebUI.getAttribute(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_RegionName'), "value")
WebUI.verifyMatch(nowActiveRegion, expectedInactiveRegion , false)
