import argparse
import os

from PIL import Image
'''
Resizes images in a directory
'''
def resizeImages(directory, outputDirectory, width, height):
    if not os.path.isdir(directory):
        return "Not directory!: " + directory
    
    for file in os.listdir(directory):
        abspath = os.path.join(directory, file)
        
        img = Image.open(abspath)
        img = img.resize((width, height), Image.ANTIALIAS)
        img.save(os.path.join(outputDirectory, os.path.join(outputDirectory, file)))


def parseArgs():
    parser = argparse.ArgumentParser(description="Parses args")
    parser.add_argument('--dir', action="store", dest="directory", required=True)
    parser.add_argument('--outputDir', action="store", dest="outputDir", required=True)
    parser.add_argument('--width', type=int, action='store', dest="width", required=True)
    parser.add_argument('--height', type=int, action='store', dest="height", required=True)

    return parser.parse_args()


if __name__ == '__main__':
    results = parseArgs()

    resizeImages(
        directory=results.directory, 
        outputDirectory=results.outputDir,
        width=results.width, 
        height=results.height
    )