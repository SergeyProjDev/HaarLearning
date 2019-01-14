import serial
import serial.tools.list_ports as port_list
import cv2
import os
import urllib.request
import urllib2
import numpy as np
import csv
import matplotlib.pyplot as plt

from tkinter import *
from PIL import Image, ImageOps

forever = True


#get serialization and parce it (url)
with open('data.csv', newline='') as f:
    reader = csv.reader(f)
    for row in reader:
        url = str(row)
url = url.replace("[", "")
url = url.replace("]", "")
url = url.replace("'", "")


#select url (getting key)
print ("Start IpWebcam on your phone!\n")
print ("  Press:\n 1. http://", url, 
			   "\n 2. Enter new url")

#reading ip
while forever:
	#getting value
	try:
		key = int(input ())
	except:
		print ("Incorrect value! Try one more time!")
		continue

	# 2 - new value and serialize it
	if key==2: 
		url = input("Enter url: http://")
		with open('data.csv', 'w', newline='') as a:
			writer = csv.writer(a)
			writer.writerow([url])
		break

	# 1 - take existed
	if key==1:
		print ("IP not found! Try one more time!")
		continue

	# wrong value
	else:
		print ("Incorrect value! Try one more time!")
		continue

# true url value of shot.jpg
url = "http://"+url+"/shot.jpg"


#GET all COM ports
ports = list(port_list.comports())
for p in ports:
    print ("   ", p)


#input Port number
print ()
Port = int(input ("Enter ONLY port NUMBER:   COM"))


#init Port
arduinoSerialData = serial.Serial()
arduinoSerialData.port = "COM"+str(Port)
arduinoSerialData.baudrate = 9600
arduinoSerialData.timeout = 1
arduinoSerialData.setDTR(False)
arduinoSerialData.open()


hand_cascade = cv2.CascadeClassifier('myhaar.xml')


#each img
while forever:
	#Get img
	imgResp = urllib.request.urlopen(url)
	imgNp = np.array(bytearray(imgResp.read()), dtype=np.uint8)
	cvimg = cv2.imdecode(imgNp, -1)

	#Mirror
	im_pil = Image.fromarray(cvimg)
	im_pil = ImageOps.mirror(im_pil)
	cvimg = np.array(im_pil)

	#analys
	gray = cv2.cvtColor(cvimg, cv2.COLOR_RGB2GRAY)
	hand=hand_cascade.detectMultiScale(gray, 1.15, 20)
	count = 0
	for (x, y, w, h) in hand:
		count = count + 1
		cv2.rectangle(cvimg, (x,y), (x+w, y+h), (0,0,255), 5)
		roi_gray = gray[y: y+h, x:x+w]
		roi_color = cvimg[y: y+h, x:x+w]
	arduinoSerialData.write(str.encode(str(count)+"\n"))
	
	#show
	cv2.imshow('test', cvimg)
	cv2.waitKey(10)


print ("ALL GONE OK")
os.system("pause")