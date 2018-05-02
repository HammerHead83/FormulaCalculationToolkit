# FormulaCalculationToolkit
Create simple evaluated expressions
For example, You want to calculate value
## { f(x) = [age](age)(age) + 0.1 * [salary](salary) - Cos( [angle1](angle1)(angle1) ) * Sin ( [angle2](angle2) ) + [age](age)(age) / Tanh( [angle1](angle1)(angle1) ) }
with **age = 25**, **salary=10000**, **angle1=0.33** and **angle2=0.66**

First of all You can create a dictionary for parameters:
{{
var values = new Dictionary<string, string>();
values.Add("age", "25");
values.Add("salary", "10000");
values.Add("angle1", "0.33");
values.Add("angle2", "0.66");
}}
In second Your formula as string must be
{{ string fx = [age](age)(age) + 0.1 * [salary](salary) - cos( [angle1](angle1)(angle1) ) * sin ( [angle2](angle2) ) + [age](age)(age) / tanh([angle1](angle1)(angle1)); }}
Formula string are caseinsensitive.
Parenthesis separator is currently not supported. It reserved for future.
At the end create new instance of **FXEngine** class. Invoke method **Evaluate** and puts there instead of **brSeparator** parameter {{ null }}.
