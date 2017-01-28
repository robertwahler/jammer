module BasicUnity

  class Run < Thor
    namespace :run
    include Thor::Actions
    include BasicUnity::BasicUnityHelper

    # adds :quiet, :skip, :pretent, :force
    add_runtime_options!

    class_option :development, :type => :boolean, :aliases => "-d", :desc => "Set the development or production stage"
    class_option :debug, :aliases => "-d", :desc => "Turn on debug logging"

    desc "desktop", "run desktop app in the build folder"
    def desktop
      set_instance_variables
      @opt = @opt + "--settings-folder #{settings_folder(@product, @stage)}"
      if mac?
        app = File.join(BUILD_FOLDER, "#{@product.capitalize}.app")
        command = "open #{app} --args #{@opt}"
      else
        # windows
        app = File.join(BUILD_FOLDER, "#{@product.capitalize}.Windows", "#{@product.capitalize}.exe")
        command = "#{app} #{@opt}"
      end
      run(command)
    end

    desc "unity", "Run the Unity IDE interactively based on the contents '.unity'"
    def unity
      invoke("unity:IDE")
    end

    desc "IDE", "Run the Unity IDE interactively based on the contents '.unity'"
    def IDE
      invoke("unity:IDE")
    end

    private

    def set_instance_variables
      @product = product_code
      @stage = options[:development] ? "development" : default_stage

      @opt = ""
      @opt = @opt + '--debug ' if options[:debug]
    end

  end
end
