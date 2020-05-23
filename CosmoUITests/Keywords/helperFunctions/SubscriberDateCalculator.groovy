package helperFunctions

import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.checkpoint.Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling
import com.kms.katalon.core.testcase.TestCase
import com.kms.katalon.core.testdata.TestData
import com.kms.katalon.core.testobject.TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import internal.GlobalVariable
import java.util.Formatter.DateTime
import java.util.Date;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.text.ParseException;


//This is a helper class to calculate dates for the subscribers
public class SubscriberDateCalculator {


	///This method will be used in the HomPage tests to calculate the expected dates.
	///This method is dynamic in the sense that it takes into consideration todays date, this means
	///All dates being displayed cannot be in the past
	@Keyword
	def ArrayList<Date> GetExpectedDates(int frequency, Date firstDate) {
		//Initialize a new list to return once populated
		ArrayList<Date> expectedDates = new ArrayList<Date>();
		Date today = new Date(System.currentTimeMillis());
		Date newDate = firstDate;

		//Set the calendar to the firstDate
		Calendar c = Calendar.getInstance();
		c.setTime(newDate);


		//Get newDate set to a valid date
		while (today.after(newDate))
		{
			c.add(Calendar.DAY_OF_MONTH, (frequency*7))
			newDate = c.getTime();
		}

		//Add the dates to the list to return
		for (int i = 0; i < 3; i++)
		{
			newDate = c.getTime();
			expectedDates.add(newDate);
			c.add(Calendar.DAY_OF_MONTH, (frequency*7))
		}


		return expectedDates;
	}

}


