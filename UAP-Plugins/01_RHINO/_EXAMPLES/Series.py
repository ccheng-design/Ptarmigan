import rhinoscriptsyntax as rs

#Use definision. Requires the variable in the paranthesis
def series_component(start, step, count):
    #use return to give back the value to the definition
    return list(range(start, start + count * step, step))


start = rs.GetInteger("Start value", 1)
step = rs.GetInteger("Step value", 1)
count = rs.GetInteger("Count", 10)

result = series_component(start, step, count)

print(result)