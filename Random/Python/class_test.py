import classes

m1 = classes.Math(8,2)
m2 = classes.Math(5,6)

m1.ShowVals()
m2.ShowVals()

print("")

print(m2.GetPow(m1.val1,m1.val2))
print(m1.GetPow(m2.val1,m2.val2))


eq = classes.Equations(2,4,9)
print(eq.GetSquaredRoot(eq.val3))
print(eq.ShowVals())