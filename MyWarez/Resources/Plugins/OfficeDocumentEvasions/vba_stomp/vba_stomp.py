import argparse
from glob import iglob
import os
import pathlib
import shutil
import tempfile
import zipfile

import olefile


def stomp_file(original_file):
    stomped_file = original_file + '.stomped'
    if olefile.isOleFile(original_file):
        shutil.copyfile(original_file, stomped_file)
        stomp_ole(stomped_file)
        return True
    elif zipfile.is_zipfile(original_file):
        tmpdir = tempfile.TemporaryDirectory(prefix='stomp_')
        with zipfile.ZipFile(original_file) as zf:
            zf.extractall(tmpdir.name)

            file_list = [f for f in iglob(tmpdir.name + '/**/*', recursive=True) if os.path.isfile(f)]
            for f in file_list:
                if olefile.isOleFile(f):
                    stomp_ole(f)

            os.chdir(pathlib.Path(stomped_file).resolve().parent)
            shutil.make_archive(stomped_file, 'zip', tmpdir.name)
            if os.path.exists(stomped_file):
                os.remove(stomped_file)
            os.rename(stomped_file + '.zip', stomped_file)
        return True
    else:
        return False


def get_macro_index(data):
    marker = b'Attrib'
    if marker in data:
        return data.rindex(marker)
    return None


def stomp_ole(ole_file):
    with olefile.OleFileIO(ole_file, write_mode=True) as ole:
        for stream_name in ole.listdir():
            data = ole.openstream(stream_name).read()
            macro_start_index = get_macro_index(data)

            if macro_start_index is None:
                continue

            non_macro_data = data[:macro_start_index]
            macro_length = len(data) - macro_start_index
            garbage = os.urandom(macro_length)

            ole.write_stream(stream_name, non_macro_data + garbage)


if __name__ == "__main__":
    argument_parser = argparse.ArgumentParser()
    argument_parser.add_argument('file', help='File to be stomped')
    args = argument_parser.parse_args()

    success = stomp_file(args.file)

    if success:
        print('[*] Stomped VBA - new file at: ' + str(args.file + '.stomped'))
    else:
        print('[!] Failed to stomp file: ' + args.file)
