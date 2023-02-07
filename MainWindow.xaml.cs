using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Big_Arithmetic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            sumOut.Text = "";
            productOut.Text = "";
            errorsOut.Text = "";

            //Unformatting text, restricting the string to only numbers.
            String unformatted1 = Regex.Replace(input1.Text, "[^0-9]", "");
            String unformatted2 = Regex.Replace(input2.Text, "[^0-9]", "");

            //Error-checking input.
            if (unformatted1 == String.Empty || unformatted2 == String.Empty)
            {
                errorsOut.Text = "PLEASE ENTER A CORRECT (INTEGER!!!) INPUT FOR EACH BOX.";
                return;
            }

            //Creating a 2D array for stacking the two large inputs, using a method to do so.
            String[,] largeStack = stackNumbers(unformatted1, unformatted2);

            //Using my big addition algorithm, taking the 2D array and length of one row, or 2D array length / 2 since theres always two rows.
            sumOut.Text = bigAddition(largeStack, (largeStack.Length / 2));

            //Using my big multiplication algorithm, taking the same inputs as big addition.
            productOut.Text = bigMultiplication(largeStack, (largeStack.Length / 2));
        }

        String[,] stackNumbers(String number1, String number2)
        {
            /*
             * My method for stacking two large numbers into a large two row 2D array to make it easier to add or multiply.
             * 
             * Looks for the largest number overall as it would dominate the amount of digits, making it the max length of the array.
             * Loops each character of the large number into each array space in the first row.
             * Pads the smaller number to the left with zeroes so its digits would align with the larger number at the end.
             * Loops each character of the small number into each array space in the second row, following the padding.
             * Returns the 2D array.
             */

            String largerNum;
            String smallerNum;
            int largeNumLength;
            int smallNumLength;

            //Determines if a larger than another, storing them respecively in largerNum or smallerNum.
            if (number1.Length >= number2.Length)
            {
                largerNum = number1;
                smallerNum = number2;
            }
            else
            {
                largerNum = number2;
                smallerNum = number1;
            }

            //Calculating lengths of the smaller and larger num.
            largeNumLength = largerNum.Length;
            smallNumLength = smallerNum.Length;

            //Creating a 2D array of the stack of the larger and smaller number, with a row length according to the larger number's length.
            String[,] numberStack = new String[2, largeNumLength];

            //Process for padding the smaller number to line up with the larger number's digits.
            if (largeNumLength - smallNumLength != 0)
            {
                for (int i = 0; i < (largeNumLength - smallNumLength); i++)
                {
                    smallerNum = "0" + smallerNum;
                }
            }

            //Adding each digit of the larger number into the first row of the 2D array, and the smaller number in the second row.
            for (int i = 0; i < largeNumLength; i++)
            {
                numberStack[0, i] = largerNum.Substring(i, 1);
                numberStack[1, i] = smallerNum.Substring(i, 1);
            }

            return numberStack;
        }

        String bigAddition(String[,] stack, int rowLength)
        {
            /*
             * My method for adding two extremely large numbers together.
             * 
             * Basic rundown:
             * Using the stacked 2D array to add each digit together.
             * Ensuring extra digits are carried over to the next digit.
             */

            String result = "";
            int tempCarry = 0;

            //Traversing throughout the entire row length of the array (aka, the amount of digits in the number)
            for (int i = rowLength - 1; i >= 0; i--)
            {
                int tempInt1; //Temp int variables
                int tempInt2; //for adding each digit together.

                Int32.TryParse(stack[0, i], out tempInt1); //Actually parsing each character
                Int32.TryParse(stack[1, i], out tempInt2); //from the array into a string.

                int sum = tempInt1 + tempInt2; //Calculating the sum of the digits.


                if ((sum % 10) + tempCarry >= 10) //Provison if a sum's one place plus a carry is or is over ten, needing another carry.
                {
                    sum = ((sum % 10) + tempCarry) % 10; //Calculates the sum of the sum and carry mod ten for the new ones place.
                    tempCarry += ((sum % 10) + tempCarry) / 10; //Calculates the new carry, appending it to the currrent carry.
                }
                else
                {
                    sum = (sum % 10) + tempCarry; //Adding the ones place of the sum (guaranteed by mod 10) and a possible carry number.
                    tempCarry = (tempInt1 + tempInt2) / 10; //Calculates a carry number with the sum divided by 10 to get the tens place, storing it in tempCarry.
                }

                result = sum.ToString() + result;
            }

            if (tempCarry > 0) //Provision for when the sum of two numbers result in a higher place.
            {
                result = tempCarry.ToString() + result; //Adding the remaining carry to the result.
            }

            return result;
        }

        String bigMultiplication(String[,] stack, int rowLength)
        {
            /*
             * My method for multiplying two extremely large numbers together. This was painful.
             * 
             * Basic rundown:
             * Using the stacked 2D array to multiply each digit of a top digit with every bottom digit.
             * Ensuring digits are carried over when a multiplication results in one.
             * When the first two multiplications are completed, each one is in a tempAdd string.
             * The stackNumbers method is used to stack the strings into a 2D array, then using bigAddition to add them together.
             * This is to ensure ease by continually adding each new multiplication into a sum instead of adding them all at once.
             */

            String result = "";
            int tempCarry = 0;

            String tempAdd1 = "";
            String tempAdd2 = "";
            String tempZeroes = "";

            //Traversing each digit in the mutliplicand, to multiply a digit in each multiplier in a nested loop.
            for (int i = rowLength - 1; i >= 0; i--)
            {
                int tempInt1;
                Int32.TryParse(stack[0, i], out tempInt1); //Parsing the digit in the multiplicand.

                /*
                 * If statement used to add all subsequent multiplications into tempAdd2, and storing the sum into tempAdd1,
                 * while during the first iteration of i, the first multiplication is stored in tempAdd1, for it to be able 
                 * to be added with tempAdd2 in the next iteration.
                 */
                if(i != rowLength - 1)
                {
                    tempAdd2 = "";
                    tempZeroes += "0";
                    tempCarry = 0;

                    //Traversing each digit in the multiplier, to be multiplied with the multiplicand i.
                    for (int j = rowLength - 1; j >= 0; j--)
                    {
                        int tempInt2;
                        Int32.TryParse(stack[1, j], out tempInt2); //Parsing the digit in the multiplier.

                        int product;

                        if ((tempInt1 * tempInt2 % 10) + tempCarry >= 10) //If statement in the case that the sum of the ones digit and a carry is over ten.
                        {
                            product = ((tempInt1 * tempInt2 % 10) + tempCarry) % 10; //Product is the ones digit of the sum of the ones digit and carry.
                            tempCarry = (tempInt1 * tempInt2 + tempCarry) / 10; //The new carry is the tens digit (/10) of the sum of the product plus carry.
                        }
                        else
                        {
                            product = (tempInt1 * tempInt2 % 10) + tempCarry; //Product is the ones digit of the product plus a potential carry.
                            tempCarry = (tempInt1 * tempInt2) / 10; //The carry is the tens digit of a product, if there is one.
                        }

                        tempAdd2 = product.ToString() + tempAdd2; //Appending each new calculated digit into a tempAdd string.
                    }

                    if (tempCarry > 0) //For when the loop is finished for the multiplier digit, but there is still a carry.
                    {
                        tempAdd2 = tempCarry + tempAdd2 + tempZeroes; //Adds the carry and rest of number, plus zeros as added in normal multiplication.
                    }
                    else
                    {
                        tempAdd2 = tempAdd2 + tempZeroes; //Adds zeroes to the right of the calculated product, with more added each time after every i.
                    }

                    String[,] tempStack = stackNumbers(tempAdd1, tempAdd2); //Stacks the two tempAdd strings.
                    tempAdd1 = bigAddition(tempStack, (tempStack.Length / 2)); //Adds the two tempAdd strings together for efficiency in each loop.
                }
                else //WORKS EXACTLY THE SAME AS ABOVE, EXCEPT STORING THE PRODUCT INTO tempAdd1 FOR IT TO BE SUMMED WITH THE CALCULATED PRODUCT IN THE NEXT ITERATION.
                {
                    tempCarry = 0;

                    for (int j = rowLength - 1; j >= 0; j--)
                    {
                        int tempInt2;
                        Int32.TryParse(stack[1, j], out tempInt2);

                        int product;

                        if ((tempInt1 * tempInt2 % 10) + tempCarry >= 10)
                        {
                            product = ((tempInt1 * tempInt2 % 10) + tempCarry) % 10;
                            tempCarry = (tempInt1 * tempInt2 + tempCarry) / 10;
                        }
                        else
                        {
                            product = (tempInt1 * tempInt2 % 10) + tempCarry;
                            tempCarry = (tempInt1 * tempInt2) / 10;
                        }

                        tempAdd1 = product.ToString() + tempAdd1;
                    }

                    if (tempCarry > 0)
                    {
                        tempAdd1 = tempCarry + tempAdd1 + tempZeroes;
                    }
                    else
                    {
                        tempAdd1 = tempAdd1 + tempZeroes;
                    }
                }
            }

            if(tempAdd1.Substring(0,1).Equals("0")) //Removes an extraneous zero from the final number.
            {
                result = tempAdd1.Substring(1);
            }
            else
            {
                result = tempAdd1;
            }
            
            return result;
        }

        private void input1_TextChanged(object sender, TextChangedEventArgs e)
        {
            sumOut.Text = "";
            productOut.Text = "";
            errorsOut.Text = "";
        }

        private void input2_TextChanged(object sender, TextChangedEventArgs e)
        {
            sumOut.Text = "";
            productOut.Text = "";
            errorsOut.Text = "";
        }
    }
}