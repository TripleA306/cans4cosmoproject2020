import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject

import java.time.LocalDate
import java.time.format.DateTimeFormatter

import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.logging.KeywordLogger
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable

/**
 * This tests the functionality of editing a Region Frequency
 * Authors: Aaron Atkinson cst201
 */

String newRegionFrequency = "4"
String orgRegionFrequency = "10"


String RegionName = "Kyle's S90 Active Region"


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
WebUI.click(findTestObject('Admin_Regions/RegionTable/td_item2'))


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

//Click on the edit Region Button
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_EditRegion'))


//Get the original next date for Region
String orgFirstCollection = WebUI.getAttribute(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_FirstCollection'), "value")
String orgNextDate =  WebUI.getText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionDate'))

int firstDay = Integer.parseInt(orgFirstCollection.substring(8,10));
int firstMonth = Integer.parseInt(orgFirstCollection.substring(5,7));
int firstYear = Integer.parseInt(orgFirstCollection.substring(0,4));

orgFirstCollection = firstMonth + "/" + firstDay + "/" + firstYear

String expectedRegionNextDate = ""

//If the nextDate and Today matches, the first collection is in the future
if(orgNextDate.equals(orgFirstCollection))
{
	expectedRegionNextDate = orgFirstCollection;
}
else
{
	//Get the first Collection date and add the days based on its frequency
	LocalDate RegionNextDate = LocalDate.of(firstYear, firstMonth, firstDay).plusDays(7 * Integer.parseInt(newRegionFrequency))
	DateTimeFormatter localeFormat = DateTimeFormatter.ofPattern("M/d/yyyy")
	expectedRegionNextDate = RegionNextDate.format(localeFormat).toString()
}


//Verify the correct information is being displayed
String entryValue = WebUI.getAttribute(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), "value")
WebUI.verifyMatch(entryValue,orgRegionFrequency , false)

//Set the Region Frequency to the new Value
WebUI.setText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), newRegionFrequency)
//Click on the save changes button
WebUI.click(findTestObject('Object Repository/Admin_Regions/Buttons/Details/btn_SaveChanges'))

Thread.sleep(1000);

//Verify the table has the updated Field
String itemFreq =  WebUI.getAttribute(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/entry_Frequency'), "value")
WebUI.verifyMatch(itemFreq,newRegionFrequency , false)

//Verify the Next Collection Date changed to what we expected
String newNextDate = WebUI.getText(findTestObject('Object Repository/Admin_Regions/DetailPaneFields/lbl_NextCollectionDate'))
WebUI.verifyMatch(newNextDate,expectedRegionNextDate, false)