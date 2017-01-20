module BasicUnity

  class Package < Thor
    namespace :package
    include Thor::Actions
    include BasicUnity::BasicUnityHelper
    include BasicUnity::PackageHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    class_option :development, :type => :boolean, :aliases => "-d", :desc => "Development options, includes debug symbols, etc"

    desc "clean", "remove tmp staging folder"
    def clean
      remove_dir(STAGING_FOLDER)
    end

    desc "linux", "create distribution package for Linux"
    method_option :tar, :type => :boolean, :desc => "Create a tar.gz container instead of a zip file"
    def linux
      set_instance_variables

      # clean staging folder
      clean

      linux_copy_to_staging(@product, options)

      if options[:tar]
        source = STAGING_FOLDER
        version = read_version_string(File.join(source, "version.txt") )
        destination = "#{PKG_FOLDER}/#{@product}.#{version}.linux.tar.gz"
        remove_file(destination)
        in_path source do
          run "tar cvzf #{destination} #{@product}"
        end
      else
        source = File.join(STAGING_FOLDER, @product)
        version = read_version_string(File.join(source, "version.txt") )
        destination = "#{PKG_FOLDER}/#{@product}.#{version}.linux.zip"
        remove_file(destination)
        run "7za a -mx9 #{destination} #{source}"
      end

    end

    desc "windows", "create distribution package for Windows"
    def windows
      set_instance_variables

      # clean staging folder
      clean

      windows_copy_to_staging(@product, options)

      # create the archive file
      source = File.join(STAGING_FOLDER, @product)
      version = read_version_string(File.join(source, "version.txt") )
      destination = "#{PKG_FOLDER}/#{@product}.#{version}.windows.zip"
      remove_file(destination)
      run "7za a -mx9 #{destination} #{source}"
    end

    desc "osx", "create distribution package for Mac OS X"
    method_option :dmg, :type => :boolean, :desc => "Create a DMG container instead of a zip file"
    def osx
      set_instance_variables

      # clean staging folder
      clean

      osx_copy_to_staging(@product)

      # get version info directly from JSON in staging folder in case this was build off a different version
      version_filename = File.join(STAGING_FOLDER, @product, 'version.txt')
      version = read_version_string(version_filename)

      if options[:dmg]
        # create the dmg file
        source = File.join(STAGING_FOLDER, @product, "#{@product}.app".capitalize)
        destination = "#{PKG_FOLDER}/#{@product}.#{version}.osx.dmg"
        remove_file(destination)
        run("hdiutil create #{destination} -volname #{@product} -size 200m -fs HFS+ -srcfolder #{source}")
      else
        source = File.join(STAGING_FOLDER, @product)
        destination = "#{PKG_FOLDER}/#{@product}.#{version}.osx.zip"
        remove_file(destination)
        run "7za a -mx9 #{destination} #{source}"
      end
    end

    desc "source", "create source code distribution archive"
    def source
      set_instance_variables

      # create the archive file
      source = ROOT_FOLDER
      # read version from resources
      version = read_version_string
      destination = "#{PKG_FOLDER}/#{@product}.#{version}.source.zip"
      remove_file(destination)
      run "7za a -mx9 -xr!.git -xr!pkg/ -xr!build/ -xr!tmp/ -xr!Tmp/ -xr!Library/ -xr!obj/ -xr!Temp/  -xr!Tmp.meta -xr!.DS_Store #{destination} #{source}"
    end

    private

    # where to start looking for templates, required by the template methods
    # even though we are using absolute paths
    def self.source_root
      File.dirname(__FILE__)
    end

    # common files to root of STAGING_FOLDER/distribution archive
    def copy_common(destination)
      source = File.join(ROOT_FOLDER, 'tasks', 'templates', 'dist', 'common')
      say_status :copy, "copying #{source} to '#{destination}'"
      directory(source,  destination)

      # HISTORY.txt
      copy_file(File.join(DOC_FOLDER, 'HISTORY.txt'), File.join(destination, 'HISTORY.txt'))

      # README.txt
      copy_file(File.join(DOC_FOLDER, 'README.txt'), File.join(destination, 'README.txt'))

      # TODO: verison.txt should be copied to staging folder by build process, otherwise the version could be mismatched
      # version.txt
      copy_file(File.join(RESOURCES_FOLDER, 'version.txt'), File.join(destination, 'version.txt'))
    end

    def set_instance_variables
      @product = default_product
    end

  end
end
