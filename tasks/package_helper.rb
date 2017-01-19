module BasicUnity

  module PackageHelper

    def linux_copy_to_staging(product, options = {})
      # copy the build folder
      source = File.join(BUILD_FOLDER, "#{product.capitalize}.Linux")
      destination = File.join(STAGING_FOLDER, product)
      say_status :copy, "copying #{source} to '#{destination}'"
      directory(source,  destination)

      # common files to root of STAGING_FOLDER/distribution archive
      copy_common(File.join(STAGING_FOLDER, product))

      # set app to execute
      destination = File.join(STAGING_FOLDER, product, "#{product}.x86")
      chmod(destination, 0755)
      destination = File.join(STAGING_FOLDER, product, "#{product}.x86_64")
      chmod(destination, 0755)

      # clear out .DS_Store
      remove_filespec_from_staging

      # clear out metafiles
      remove_filespec_from_staging File.join(STAGING_FOLDER, '**', '*.meta')
    end

    def windows_copy_to_staging(product, options = {})
      # copy the build folder
      source = File.join(BUILD_FOLDER, "#{product.capitalize}.Windows")
      destination = File.join(STAGING_FOLDER, product)
      say_status :copy, "copying #{source} to '#{destination}'"
      directory(source,  destination)

      # common files to root of STAGING_FOLDER/distribution archive
      copy_common(File.join(STAGING_FOLDER, product))

      # Convert to DOS EOL
      filenames = ['version.txt', 'LICENSE.txt', 'HISTORY.txt', 'OPTIONS.txt', 'SETTINGS.txt', 'README.txt']
      filenames.each do |filename|
        destination = File.join(STAGING_FOLDER, product, filename)
        run("unix2dos --quiet #{destination}")
      end

      # copy the steam dll file if this is a ateam build
      source = File.join(STAGING_FOLDER, product, "steam_appid.txt")
      if File.exists?(source)
        source = File.join(VENDOR_FOLDER, "Steamworks", "Plugins", "x86", "steam_api.dll")
        destination = File.join(STAGING_FOLDER, product, "steam_api.dll")
        say_status :copy, "copying #{source} to '#{destination}'"
        copy_file(source,  destination)
      end

      # remove symbol pdb files
      unless options[:development]
        remove_file(File.join(STAGING_FOLDER, product, "player_win_x86.pdb"))
        remove_file(File.join(STAGING_FOLDER, product, "player_win_x86_s.pdb"))
      end

      # clear out .DS_Store
      remove_filespec_from_staging

      # clear out metafiles
      remove_filespec_from_staging File.join(STAGING_FOLDER, '**', '*.meta')
    end

    def osx_copy_to_staging(product)
      # copy the build folder
      source = File.join(BUILD_FOLDER, "#{product}.app".capitalize)
      destination = File.join(STAGING_FOLDER, product, "#{product}.app".capitalize )
      say_status :copy, "copying #{source} to '#{destination}'"
      directory(source,  destination)

      # common files to the app bundle (macOS only)
      copy_common(File.join(STAGING_FOLDER, product, "#{product}.app".capitalize, 'Contents', 'MacOS'))

      # common files to root of STAGING_FOLDER/product archive
      copy_common(File.join(STAGING_FOLDER, product))

      # set app to execute
      destination = File.join(STAGING_FOLDER, product, "#{product}.app".capitalize, 'Contents', 'MacOS', product)
      chmod(destination, 0755)

      # clear out .DS_Store
      remove_filespec_from_staging

      # clear out metafiles
      remove_filespec_from_staging File.join(STAGING_FOLDER, '**', '*.meta')
    end


    def remove_filespec_from_staging(filespec=nil)
      filespec = File.join(STAGING_FOLDER, '**', '.DS_Store') unless filespec
      Dir.glob(filespec).each do |filename|
        remove_file(filename)
      end
    end

  end
end
