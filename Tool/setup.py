import os
import commands

def find_newest(path):
    """Recursively find the newest modified time of any file in the hierarchy"""
    if not os.path.exists(path):
        return 0
    
    newest_time = 0
    for item in os.listdir(path):
        item_path = os.path.join(path, item)
        if os.path.isfile(item_path):
            item_time = os.path.getmtime(item_path)
        else:
            item_time = find_newest(item_path)
        if item_time > newest_time:
            newest_time = item_time
    return newest_time

if find_newest("../Localization") < find_newest("."):
    commands.make_files()
    commands.setup()